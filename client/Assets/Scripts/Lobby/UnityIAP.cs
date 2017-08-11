using UnityEngine;

public class UnityIAP : MonoBehaviour {

    private static UnityIAPStoreListener m_Singleton;

    void Start () {
        if (m_Singleton == null)
        {
            m_Singleton = new UnityIAPStoreListener();
            m_Singleton.InitializeIAP();
        }
    }

    public void PurchaseButton ()
    {
        string _target = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        switch (_target)
        {
            case "Button01":
                m_Singleton.PurchaseItem(0);
                break;
            case "Button02":
                m_Singleton.PurchaseItem(1);
                break;
            case "Button03":
                m_Singleton.PurchaseItem(2);
                break;
            case "Button04":
                m_Singleton.PurchaseItem(3);
                break;
            case "Button05":
                m_Singleton.PurchaseItem(4);
                break;
            case "Button06":
                m_Singleton.PurchaseItem(5);
                break;
        }
    }

}
