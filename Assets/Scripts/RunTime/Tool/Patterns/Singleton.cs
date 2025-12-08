using System;
using System.Threading;

/// <summary>
///     简单的单例模式泛型基类
/// </summary>
/// <typeparam name="T">要创建单例的类型</typeparam>
public abstract class SingletonBase<T> where T : class, new()
{
    // 使用 Lazy<T> 实现线程安全的延迟初始化
    private static readonly Lazy<T> _instance = new(() =>
                                                        Activator.CreateInstance(typeof(T), true) as T,
                                                    LazyThreadSafetyMode.ExecutionAndPublication);

    // 保护构造函数，防止外部实例化
    protected SingletonBase()
    {
        if (_instance.IsValueCreated)
            throw new InvalidOperationException("Singleton instance already created.");
    }

    // 获取单例实例的公共属性
    public static T Instance
    {
        get => _instance.Value;
    }
}