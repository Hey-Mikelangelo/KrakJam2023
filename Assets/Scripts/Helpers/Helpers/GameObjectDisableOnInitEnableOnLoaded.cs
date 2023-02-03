public class GameObjectDisableOnInitEnableOnLoaded : LoadableMonoBehaviour
{
    public override void OnInit()
    {
        base.OnInit();
        this.gameObject.SetActive(false);
    }
    public override void OnLoaded()
    {
        base.OnLoaded();
        this.gameObject.SetActive(true);
    }
}