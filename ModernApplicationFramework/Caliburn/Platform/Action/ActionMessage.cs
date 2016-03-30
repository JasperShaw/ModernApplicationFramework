﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Logger;
using ModernApplicationFramework.Caliburn.Result;
using EventTrigger = System.Windows.Interactivity.EventTrigger;
using IHaveParameters = ModernApplicationFramework.Caliburn.Platform.Interfaces.IHaveParameters;
using MessageBinder = ModernApplicationFramework.Caliburn.Platform.Utilities.MessageBinder;
using View = ModernApplicationFramework.Caliburn.Platform.Xaml.View;

namespace ModernApplicationFramework.Caliburn.Platform.Action
{
    [ContentProperty("Parameters")]
    [DefaultTrigger(typeof (FrameworkElement), typeof (EventTrigger), "MouseLeftButtonDown")]
    [DefaultTrigger(typeof (ButtonBase), typeof (EventTrigger), "Click")]
    [TypeConstraint(typeof (FrameworkElement))]
    public class ActionMessage : TriggerAction<FrameworkElement>, IHaveParameters
    {
        static readonly ILog Log = LogManager.GetLog(typeof (ActionMessage));
        ActionExecutionContext _context;

#if WINDOWS_PHONE
        internal Microsoft.Phone.Shell.IApplicationBarMenuItem applicationBarSource;
#endif

        internal static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached(
            "Handler",
            typeof (object),
            typeof (ActionMessage),
            new PropertyMetadata(null, HandlerPropertyChanged)
            );

        ///<summary>
        /// Causes the action invocation to "double check" if the action should be invoked by executing the guard immediately before hand.
        ///</summary>
        /// <remarks>This is disabled by default. If multiple actions are attached to the same element, you may want to enable this so that each individaul action checks its guard regardless of how the UI state appears.</remarks>
        public static bool EnforceGuardsDuringInvocation = false;

        ///<summary>
        /// Causes the action to throw if it cannot locate the target or the method at invocation time.
        ///</summary>
        /// <remarks>True by default.</remarks>
        public static bool ThrowsExceptions = true;

        /// <summary>
        /// Represents the method name of an action message.
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(
                "MethodName",
                typeof (string),
                typeof (ActionMessage),
                null
                );

        /// <summary>
        /// Represents the parameters of an action message.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register(
                "Parameters",
                typeof (Utilities.AttachedCollection<Parameter>),
                typeof (ActionMessage),
                null
                );

        /// <summary>
        /// Creates an instance of <see cref="ActionMessage"/>.
        /// </summary>
        public ActionMessage()
        {
            SetValue(ParametersProperty, new Utilities.AttachedCollection<Parameter>());
        }

        /// <summary>
        /// Gets or sets the name of the method to be invoked on the presentation model class.
        /// </summary>
        /// <value>The name of the method.</value>
#if !WinRT
        [Category("Common Properties")]
#endif
            public string MethodName
        {
            get { return (string) GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        /// <summary>
        /// Gets the parameters to pass as part of the method invocation.
        /// </summary>
        /// <value>The parameters.</value>
#if !WinRT
        [Category("Common Properties")]
#endif
            public Utilities.AttachedCollection<Parameter> Parameters
        {
            get { return (Utilities.AttachedCollection<Parameter>) GetValue(ParametersProperty); }
        }

        /// <summary>
        /// Occurs before the message detaches from the associated object.
        /// </summary>
        public event EventHandler Detaching = delegate { };

        /// <summary>
        /// Called after the action is attached to an AssociatedObject.
        /// </summary>
#if WinRT81
        protected override void OnAttached() {
            if (!View.InDesignMode) {
                Parameters.Attach(AssociatedObject);
                Parameters.OfType<Parameter>().Apply(x => x.MakeAwareOf(this));

                
                if (View.ExecuteOnLoad(AssociatedObject, ElementLoaded)) {
                    // Not yet sure if this will be needed
                    //var trigger = Interaction.GetTriggers(AssociatedObject)
                    //    .FirstOrDefault(t => t.Actions.Contains(this)) as EventTrigger;
                    //if (trigger != null && trigger.EventName == "Loaded")
                    //    Invoke(new RoutedEventArgs());
                }

                View.ExecuteOnUnload(AssociatedObject, ElementUnloaded);
            }

            base.OnAttached();
        }

        void ElementUnloaded(object sender, RoutedEventArgs e)
        {
            OnDetaching();
        }
#else
        protected override void OnAttached()
        {
            if (!View.InDesignMode)
            {
                Parameters.Attach(AssociatedObject);
                Parameters.Apply(x => x.MakeAwareOf(this));

                if (View.ExecuteOnLoad(AssociatedObject, ElementLoaded))
                {
                    var trigger = Interaction.GetTriggers(AssociatedObject)
                        .FirstOrDefault(t => t.Actions.Contains(this)) as EventTrigger;
                    if (trigger != null && trigger.EventName == "Loaded")
                        Invoke(new RoutedEventArgs());
                }
            }

            base.OnAttached();
        }
#endif

        static void HandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ActionMessage) d).UpdateContext();
        }

        /// <summary>
        /// Called when the action is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if (!View.InDesignMode)
            {
                Detaching(this, System.EventArgs.Empty);
                AssociatedObject.Loaded -= ElementLoaded;
                Parameters.Detach();
            }

            base.OnDetaching();
        }

        void ElementLoaded(object sender, RoutedEventArgs e)
        {
            UpdateContext();

            DependencyObject currentElement;
            if (_context.View == null)
            {
                currentElement = AssociatedObject;
                while (currentElement != null)
                {
                    if (Action.HasTargetSet(currentElement))
                        break;

                    currentElement = VisualTreeHelper.GetParent(currentElement);
                }
            }
            else currentElement = _context.View;

            var binding = new Binding
            {
                Path = new PropertyPath(Message.HandlerProperty),
                Source = currentElement
            };

            BindingOperations.SetBinding(this, HandlerProperty, binding);
        }

        void UpdateContext()
        {
            _context?.Dispose();

            _context = new ActionExecutionContext
            {
                Message = this,
                Source = AssociatedObject
            };

            PrepareContext(_context);
            UpdateAvailabilityCore();
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="eventArgs">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object eventArgs)
        {
            Log.Info("Invoking {0}.", this);

            if (_context == null)
            {
                UpdateContext();
            }

            if (_context != null && (_context.Target == null || _context.View == null))
            {
                PrepareContext(_context);
                if (_context.Target == null)
                {
                    var ex = new Exception($"No target found for method {_context.Message.MethodName}.");
                    Log.Error(ex);

                    if (!ThrowsExceptions)
                        return;
                    throw ex;
                }

                if (!UpdateAvailabilityCore())
                {
                    return;
                }
            }

            if (_context != null && _context.Method == null)
            {
                var ex =
                    new Exception(
                        $"Method {_context.Message.MethodName} not found on target of type {_context.Target.GetType()}.");
                Log.Error(ex);

                if (!ThrowsExceptions)
                    return;
                throw ex;
            }

            if (_context == null)
                return;
            _context.EventArgs = eventArgs;

            if (EnforceGuardsDuringInvocation && _context.CanExecute != null && !_context.CanExecute())
            {
                return;
            }

            InvokeAction(_context);
            _context.EventArgs = null;
        }

        /// <summary>
        /// Forces an update of the UI's Enabled/Disabled state based on the the preconditions associated with the method.
        /// </summary>
        public virtual void UpdateAvailability()
        {
            if (_context == null)
                return;

            if (_context.Target == null || _context.View == null)
                PrepareContext(_context);

            UpdateAvailabilityCore();
        }

        bool UpdateAvailabilityCore()
        {
            Log.Info("{0} availability update.", this);
            return ApplyAvailabilityEffect(_context);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Action: " + MethodName;
        }

        /// <summary>
        /// Invokes the action using the specified <see cref="ActionExecutionContext"/>
        /// </summary>
        public static Action<ActionExecutionContext> InvokeAction = context =>
        {
            var values = MessageBinder.DetermineParameters(context, context.Method.GetParameters());
            var returnValue = context.Method.Invoke(context.Target, values);

            var task = returnValue as System.Threading.Tasks.Task;
            if (task != null)
            {
                returnValue = task.AsResult();
            }

            var result = returnValue as IResult;
            if (result != null)
            {
                returnValue = new[] {result};
            }

            var enumerable = returnValue as IEnumerable<IResult>;
            if (enumerable != null)
            {
                returnValue = enumerable.GetEnumerator();
            }

            var enumerator = returnValue as IEnumerator<IResult>;
            if (enumerator != null)
            {
                Coroutine.BeginExecute(enumerator,
                    new CoroutineExecutionContext
                    {
                        Source = context.Source,
                        View = context.View,
                        Target = context.Target
                    });
            }
        };

        /// <summary>
        /// Applies an availability effect, such as IsEnabled, to an element.
        /// </summary>
        /// <remarks>Returns a value indicating whether or not the action is available.</remarks>
        public static Func<ActionExecutionContext, bool> ApplyAvailabilityEffect = context =>
        {
            var source = context.Source;
            if (source == null)
            {
                return true;
            }

            var hasBinding = ConventionManager.HasBinding(source, UIElement.IsEnabledProperty);
            if (!hasBinding && context.CanExecute != null)
            {
                source.IsEnabled = context.CanExecute();
            }

            return source.IsEnabled;
        };

        /// <summary>
        /// Finds the method on the target matching the specified message.
        /// </summary>
        /// <returns>The matching method, if available.</returns>
        public static Func<ActionMessage, object, MethodInfo> GetTargetMethod =
            (message, target) => (from method in target.GetType().GetMethods()
                where method.Name == message.MethodName
                let methodParameters = method.GetParameters()
                where message.Parameters.Count == methodParameters.Length
                select method).FirstOrDefault();

        /// <summary>
        /// Sets the target, method and view on the context. Uses a bubbling strategy by default.
        /// </summary>
        public static Action<ActionExecutionContext> SetMethodBinding = context =>
        {
            var source = context.Source;

            DependencyObject currentElement = source;
            while (currentElement != null)
            {
                if (Action.HasTargetSet(currentElement))
                {
                    var target = Message.GetHandler(currentElement);
                    if (target != null)
                    {
                        var method = GetTargetMethod(context.Message, target);
                        if (method != null)
                        {
                            context.Method = method;
                            context.Target = target;
                            context.View = currentElement;
                            return;
                        }
                    }
                    else
                    {
                        context.View = currentElement;
                        return;
                    }
                }

                currentElement = VisualTreeHelper.GetParent(currentElement);
            }

            if (source?.DataContext == null)
                return;
            {
                var target = source.DataContext;
                var method = GetTargetMethod(context.Message, target);

                if (method == null)
                    return;
                context.Target = target;
                context.Method = method;
                context.View = source;
            }
        };

        /// <summary>
        /// Prepares the action execution context for use.
        /// </summary>
        public static Action<ActionExecutionContext> PrepareContext = context =>
        {
            SetMethodBinding(context);
            if (context.Target == null || context.Method == null)
            {
                return;
            }
            var possibleGuardNames = BuildPossibleGuardNames(context.Method).ToList();

            var guard = TryFindGuardMethod(context, possibleGuardNames);

            if (guard == null)
            {
                var inpc = context.Target as INotifyPropertyChanged;
                if (inpc == null)
                    return;

                var targetType = context.Target.GetType();
                string matchingGuardName = null;
                foreach (string possibleGuardName in possibleGuardNames)
                {
                    matchingGuardName = possibleGuardName;
                    guard = GetMethodInfo(targetType, "get_" + matchingGuardName);
                    if (guard != null) break;
                }

                if (guard == null)
                    return;

                PropertyChangedEventHandler handler = null;
                handler = (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == matchingGuardName)
                    {
                        Execute.OnUiThread(() =>
                        {
                            var message = context.Message;
                            if (message == null)
                            {
                                inpc.PropertyChanged -= handler;
                                return;
                            }
                            message.UpdateAvailability();
                        });
                    }
                };

                inpc.PropertyChanged += handler;
                context.Disposing += delegate { inpc.PropertyChanged -= handler; };
                context.Message.Detaching += delegate { inpc.PropertyChanged -= handler; };
            }

            context.CanExecute = () => (bool) guard.Invoke(
                context.Target,
                MessageBinder.DetermineParameters(context, guard.GetParameters()));
        };

        /// <summary>
        /// Try to find a candidate for guard function, having: 
        ///    - a name matching any of <paramref name="possibleGuardNames"/>
        ///    - no generic parameters
        ///    - a bool return type
        ///    - no parameters or a set of parameters corresponding to the action method
        /// </summary>
        /// <param name="context">The execution context</param>
        /// <param name="possibleGuardNames">Method names to look for.</param>
        ///<returns>A MethodInfo, if found; null otherwise</returns>
        static MethodInfo TryFindGuardMethod(ActionExecutionContext context, IEnumerable<string> possibleGuardNames)
        {
            var targetType = context.Target.GetType();
            MethodInfo guard = null;
            foreach (string possibleGuardName in possibleGuardNames)
            {
                guard = GetMethodInfo(targetType, possibleGuardName);
                if (guard != null) break;
            }

            if (guard == null)
                return null;
            if (guard.ContainsGenericParameters)
                return null;
            if (!typeof (bool).Equals(guard.ReturnType))
                return null;

            var guardPars = guard.GetParameters();
            var actionPars = context.Method.GetParameters();
            if (guardPars.Length == 0) return guard;
            if (guardPars.Length != actionPars.Length) return null;

            var comparisons = guardPars.Zip(
                context.Method.GetParameters(),
                (x, y) => x.ParameterType == y.ParameterType
                );

            if (comparisons.Any(x => !x))
            {
                return null;
            }

            return guard;
        }

        /// <summary>
        /// Returns the list of possible names of guard methods / properties for the given method.
        /// </summary>
        public static Func<MethodInfo, IEnumerable<string>> BuildPossibleGuardNames = method =>
        {
            var guardNames = new List<string>();

            const string guardPrefix = "Can";

            var methodName = method.Name;

            guardNames.Add(guardPrefix + methodName);

            const string asyncMethodSuffix = "Async";

            if (methodName.EndsWith(asyncMethodSuffix, StringComparison.OrdinalIgnoreCase))
            {
                guardNames.Add(guardPrefix + methodName.Substring(0, methodName.Length - asyncMethodSuffix.Length));
            }

            return guardNames;
        };

        static MethodInfo GetMethodInfo(Type t, string methodName)
        {
            return t.GetMethod(methodName);
        }
    }
}