namespace ModernApplicationFramework.Utilities.Tests
{
    internal class Base : IBase
    {

    }

    internal class Sub : Base, ISub
    {

    }


    internal class SubSub : Sub, IOther
    {

    }


    internal interface IBase
    {

    }

    internal interface ISub : IBase
    {

    }

    internal interface IOther
    {

    }

    internal class Other : IOther
    {

    }
}