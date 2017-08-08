using System;

namespace ModernApplicationFramework.Input.Command
{
    public class GestureScope
    {
        public string Text { get; }
        
        public Guid Id { get; }
        
        public GestureScope(string guid, string text)
        {
            Id = Guid.Parse(guid);
            Text = text;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GestureScope commandGesture))
                return false;
            return Id.Equals(commandGesture.Id);
        }

        protected bool Equals(GestureScope other)
        {
            return Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}