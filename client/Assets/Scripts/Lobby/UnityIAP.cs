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
            case "item1":
                m_Singleton.PurchaseItem(0);
                break;
            case "item2":
                m_Singleton.PurchaseItem(1);
                break;
            case "item3":
                m_Singleton.PurchaseItem(2);
                break;
            case "item4":
                m_Singleton.PurchaseItem(3);
                break;
            case "item5":
                m_Singleton.PurchaseItem(4);
                break;
            case "item6":
                m_Singleton.PurchaseItem(5);
                break;
        }
    }

}
