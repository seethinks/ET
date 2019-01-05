using System;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public static class AdaptHelper
{
    public class AdaptMethod
    {
        public string Name;
        public int ParamCount;
        public IMethod Method;
    }

    public static IMethod GetMethod(this ILType type, AdaptMethod m)
    {
        if (m.Method != null)
        {
            return m.Method;
        }

        m.Method = type.GetMethod(m.Name, m.ParamCount);

        if (m.Method != null)
        {
            return m.Method;
        }

        string baseClass = "";
        if (type.FirstCLRBaseType != null)
        {
            baseClass = type.FirstCLRBaseType.FullName;
        }
        else if (type.FirstCLRInterface != null)
        {
            baseClass = type.FirstCLRInterface.FullName;
        }

        throw new Exception($"can't find the method: {type.FullName}.{m.Name}:{baseClass}, paramCount={m.ParamCount}");

    }
}

public abstract class MyAdaptor: CrossBindingAdaptorType
{
    protected AppDomain AppDomain { get; set; }
    protected ILTypeInstance instance;
    private readonly AdaptHelper.AdaptMethod[] methods;

    protected MyAdaptor(AdaptHelper.AdaptMethod[] methods)
    {
        this.methods = methods;
    }

    public ILTypeInstance ILInstance
    {
        get
        {
            return this.instance;
        }
        set
        {
            this.instance = value;
        }
    }

    protected object Invoke(int index, params object[] p)
    {
        IMethod m = this.instance.Type.GetMethod(this.methods[index]);
        return AppDomain.Invoke(m, this.instance, p);
    }

    protected MyAdaptor(AppDomain appdomain, ILTypeInstance instance, AdaptHelper.AdaptMethod[] methods)
    {
        this.methods = methods;
        AppDomain = appdomain;
        this.instance = instance;
    }
}