using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class UnityIAPStoreListener : IStoreListener
{
    // Unity IAP objects 
    private IStoreController m_Controller;
    bool purchaseInProgress = false;

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
        if (purchaseInProgress) return;

        if (m_Controller != null)
        {
            var product = m_Controller.products.all[num];
            if (product != null && product.availableToPurchase)
            {
                m_Controller.InitiatePurchase(product);
                purchaseInProgress = true;
            }
            else
            {
                Debug.Log("no available product.");
            }
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (!purchaseInProgress) return PurchaseProcessingResult.Complete;
        purchaseInProgress = false;

        Debug.Log("Purchase OK, ID = " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);

        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            int getCoin = 0;
            // Unlock the appropriate content here.
            switch (e.purchasedProduct.definition.id)
            {
                case "Beginner Pack":
                    getCoin = 5000;
                    break;
                case "Happy Pack":
                    getCoin = 10000;
                    break;
                case "Adventurer Pack":
                    getCoin = 50000;
                    break;
                case "Master Pack":
                    getCoin = 100000;
                    break;
                case "Wealth Pack":
                    getCoin = 500000;
                    break;
                case "Professional Pack":
                    getCoin = 1000000;
                    break;
            }
            com.Lobby.Launcher.instance.ChangeCoin(getCoin);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        if (!purchaseInProgress) return;
        purchaseInProgress = false;
        Debug.Log("Purchase failed: " + item.definition.id);
        //Debug.Log(r);
    }


}
