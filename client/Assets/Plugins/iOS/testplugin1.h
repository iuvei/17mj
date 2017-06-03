//
//  testplugin1.h
//  testplugin1
//
//  Created by 朱賢譯 on 2017/5/18.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <WowzaGoCoderSDK/WowzaGoCoderSDK.h>
//#import <MediaPlayer/MediaPlayer.h>
//#import <UIKit/UIKit.h>

//#import <UIKit/UIKit.h>
//extern UIViewController* UnityGetGLViewController();
//extern "C" void UnitySendMessage(const char*, const char*, const char*);

//@interface testplugin1 : NSObject
//{
//}

//@interface testplugin1 : NSObject <WZStatusCallback>
@interface testplugin1 : UIViewController <WZStatusCallback>
{
    //UnityAppController *objectUnityAppController;
    //MPMoviePlayerController *videoController;
}
// The top level GoCoder API interface
@property (nonatomic, strong) WowzaGoCoder *goCoder;
@property (nonatomic, strong) UIView* videoView;
@property (nonatomic, strong) UIView* unityView;
//@property (nonatomic, strong) UIViewController* parent;
//@property (nonatomic, strong) WowzaGoCoder *goCoder;
//@property (nonatomic, strong) UIView* myView;
//@property (nonatomic, strong) NSString* gameObjectName;
//#if __cplusplus
//extern "C" {
//#endif
//    //-(int) test;
//    -(void) initxxx;
//#if __cplusplus
//} //Extern C
//#endif
@end
