### The problem

In Visual Studio we cant load the required certificate for a the developer provisioning profile.

![cant load certificates](/docs/images/using-developer-provisioning-profile/cant-load-certificates.png)

The error mesage recommends using an **individual accound** instead.

For this reason, the code is no more deployable to the device

For this, we need some values

![API Key Information](/docs/images/using-developer-provisioning-profile/enter-your-app-api-key-information.png)

How we can create such an API key information, thats described [there](https://developer.apple.com/documentation/appstoreconnectapi/creating_api_keys_for_app_store_connect_api).

The created API key looks like

![the created API key](/docs/images/using-developer-provisioning-profile/your-api-key.png)

Store that information and enter the values into the form
![enter values into](/docs/images/using-developer-provisioning-profile/your-app-api-key-values.png)

After that, all certicates and profiles are available and selectable

![your certifcates and profiles](/docs/images/using-developer-provisioning-profile/select-your-desired-profile.png)

We can use our wildcard profile again

![using the wildcard certificate](/docs/images/debugger-provisioning.png)
