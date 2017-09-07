#import "GsPlugin.h"

static NSString * const CALLBACK_OBJECT = @"AccountManager";
static NSString * const CALLBACK_METHOD = @"OnConnected";
static NSString * const CALLBACK_RETFAIL = @"No Init, ";
static NSString * const kClientID =
    @"1017074278495-koj4lks4ip0kgacs9tain2uhr7ikb30t.apps.googleusercontent.com";

@implementation GsPlugin

static GsPlugin *_sharedInstance = nil;
static dispatch_once_t _onceToken;

+ (GsPlugin *)sharedInstance {
    dispatch_once(&_onceToken, ^{
        _sharedInstance = [[GsPlugin alloc] init];
    });
    return _sharedInstance;
}

- (void)onOpenURL:(NSNotification *)notification
{
    NSURL *url = notification.userInfo[@"url"];
    NSString *sourceApplication = notification.userInfo[@"sourceApplication"];
    id annotation = notification.userInfo[@"annotation"];
    [[GIDSignIn sharedInstance] handleURL:url
                               sourceApplication:sourceApplication
                                      annotation:annotation];
}

// Present a view that prompts the user to sign in with Google
- (void)signIn:(GIDSignIn *)signIn presentViewController:(UIViewController *)viewController {
    //[self presentViewController:viewController animated:YES completion:nil];
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:viewController animated:YES completion:nil];
}

- (void)setUp {
  [GIDSignIn sharedInstance].clientID = kClientID;

  GIDSignIn *signIn = [GIDSignIn sharedInstance];
  signIn.shouldFetchBasicProfile = YES;
  signIn.delegate = self;
  signIn.uiDelegate = self;
  UnityRegisterAppDelegateListener(self);
}


- (void)signIn:(GIDSignIn *)signIn didSignInForUser:(GIDGoogleUser *)user withError:(NSError *)error {
    NSLog(@"signIn()");
    if (user.authentication == nil) {
        UnitySendMessage([CALLBACK_OBJECT UTF8String],[CALLBACK_METHOD UTF8String],[CALLBACK_RETFAIL UTF8String]);
        return;
    }
    NSString *userId = user.userID;                  // For client-side use only!
    NSString *idToken = user.authentication.idToken; // Safe to send to the server
    NSString *email = user.profile.email;
    NSURL *imageURL = [user.profile imageURLWithDimension:100];
    NSString* ans = [NSString stringWithFormat: @"%@,%@,%@,%@", user.profile.email, user.userID, user.profile.name, imageURL ];
    UnitySendMessage([CALLBACK_OBJECT UTF8String],[CALLBACK_METHOD UTF8String],[ans UTF8String]);
}

-(void)loginCheck{
    NSLog(@"loginCheck()...");
    [self setUp];
    [[GIDSignIn sharedInstance] signInSilently];
}

-(void)login{
    NSLog(@"login()...");
    UIViewController *unityController = UnityGetGLViewController();
    [[GIDSignIn sharedInstance] signIn];
}

-(void)loginOut{
    NSLog(@"loginOut()...");
    [[GIDSignIn sharedInstance] signOut];
}
@end

#pragma mark Unity Plugin

extern "C" {
    void _loginCheck(){
        GsPlugin *obj = [GsPlugin sharedInstance];
        [obj loginCheck];
    }

    void _login(){
        GsPlugin *obj = [GsPlugin sharedInstance];
        [obj login];
    }
    
    void _loginOut(){
        GsPlugin *obj = [GsPlugin sharedInstance];
        [obj loginOut];
    }
}
