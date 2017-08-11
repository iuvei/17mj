using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class UnityIAPStoreListener : IStoreListener
{
    // Unity IAP objects 
    private IStoreController m_Controller;

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        /*
        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString
                    }));
            }
        }*/
    }
    
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK");
        //Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        //Debug.Log("Receipt: " + e.purchasedProduct.receipt);
        return PurchaseProcessingResult.Complete;
    }
        
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        Debug.Log("Purchase failed: " + item.definition.id);
        //Debug.Log(r);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public void InitializeIAP()
    {
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);

        builder.AddProduct("Beginner Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_01", GooglePlay.Name , AppleAppStore.Name}
            });
        builder.AddProduct("Happy Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_02", GooglePlay.Name , AppleAppStore.Name}
            });
        builder.AddProduct("Adventurer Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_03", GooglePlay.Name , AppleAppStore.Name}
            });
        builder.AddProduct("Master Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_04", GooglePlay.Name , AppleAppStore.Name}
            });
        builder.AddProduct("Wealth Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_05", GooglePlay.Name , AppleAppStore.Name}
            });
        builder.AddProduct("Professional Pack", ProductType.Consumable, new IDs
            {
                {"17_coin_06", GooglePlay.Name , AppleAppStore.Name}
            });

        UnityPurchasing.Initialize(this, builder);
    }

    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    public void PurchaseItem(int num)
    {
        if (m_Controller != null)
        {
            var product = m_Controller.products.all[num];
            if (product != null && product.availableToPurchase)
            {               
                m_Controller.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("no available product.");
            }
        }
    }

  
}
