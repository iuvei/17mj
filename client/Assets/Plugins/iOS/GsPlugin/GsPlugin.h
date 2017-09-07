#import "UnityAppController.h"
#import "AppDelegateListener.h"
#import "GoogleSignIn/GoogleSignIn.h"

@interface GsPlugin : UnityAppController <GIDSignInUIDelegate, GIDSignInDelegate, AppDelegateListener>
@end
