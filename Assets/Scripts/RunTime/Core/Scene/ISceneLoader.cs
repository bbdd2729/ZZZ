using Cysharp.Threading.Tasks;

public interface ISceneLoader
{
    public UniTask LoadSceneAsync(string sceneName);
}