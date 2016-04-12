using System;
using ModernApplicationFramework.Caliburn.Result;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IOpenResult<TChild> : IResult
    {
        Action<TChild> OnConfigure { get; set; }
        Action<TChild> OnShutDown { get; set; }

        //void SetData<TData>(TData data);
    }
}
