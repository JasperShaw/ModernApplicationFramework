using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Platform.Utilities;
using DependencyPropertyHelper = ModernApplicationFramework.Caliburn.Platform.Utilities.DependencyPropertyHelper;
using TriggerBase = System.Windows.Interactivity.TriggerBase;

namespace ModernApplicationFramework.Caliburn.Platform
{
    /// <summary>
    ///     Host's attached properties related to routed UI messaging.
    /// </summary>
    public static class Message
    {
        internal static readonly DependencyProperty HandlerProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Handler",
                typeof(object),
                typeof(Message));

        private static readonly DependencyProperty MessageTriggersProperty =
            DependencyPropertyHelper.RegisterAttached(
                "MessageTriggers",
                typeof(TriggerBase[]),
                typeof(Message));

        /// <summary>
        ///     A property definition representing attached triggers and messages.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Attach",
                typeof(string),
                typeof(Message),
                null,
                OnAttachChanged
                );


        /// <summary>
        ///     Gets the attached triggers and messages.
        /// </summary>
        /// <param name="d"> The element that was attached to. </param>
        /// <returns> The parsable attachment text. </returns>
        public static string GetAttach(DependencyObject d)
        {
            return d.GetValue(AttachProperty) as string;
        }

        /// <summary>
        ///     Gets the message handler for this element.
        /// </summary>
        /// <param name="d"> The element. </param>
        /// <returns> The message handler. </returns>
        public static object GetHandler(DependencyObject d)
        {
            return d.GetValue(HandlerProperty);
        }

        /// <summary>
        ///     Sets the attached triggers and messages.
        /// </summary>
        /// <param name="d"> The element to attach to. </param>
        /// <param name="attachText"> The parsable attachment text. </param>
        public static void SetAttach(DependencyObject d, string attachText)
        {
            d.SetValue(AttachProperty, attachText);
        }

        /// <summary>
        ///     Places a message handler on this element.
        /// </summary>
        /// <param name="d"> The element. </param>
        /// <param name="value"> The message handler. </param>
        public static void SetHandler(DependencyObject d, object value)
        {
            d.SetValue(HandlerProperty, value);
        }

        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            var messageTriggers = (TriggerBase[]) d.GetValue(MessageTriggersProperty);

            var allTriggers = Interaction.GetTriggers(d);

            messageTriggers?.Apply(x => allTriggers.Remove(x));

            var newTriggers = Parser.Parse(d, e.NewValue as string).ToArray();
            newTriggers.Apply(allTriggers.Add);

            if (newTriggers.Length > 0)
            {
                d.SetValue(MessageTriggersProperty, newTriggers);
            }
            else
            {
                d.ClearValue(MessageTriggersProperty);
            }
        }
    }
}