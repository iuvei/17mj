//
//  MyPlugin.h
//  MyPlugin
//
//  Created by 朱賢譯 on 2017/6/5.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//
#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <AlivcLiveVideo/AlivcLiveVideo.h>
#import "UnityAppController.h"
#import "UI/UnityView.h"
#import "UI/UnityViewControllerBase.h"
#import "myViewController.h"
#import "myxViewController.h"

//extern UIViewController *UnityGetGLViewController();
//static MyPlugin *_sharedInstance;
@interface MyPlugin : UnityAppController <AlivcLiveSessionDelegate>;
//static MyPlugin *_sharedInstance;
//@property (nonatomic, strong) myViewController *presentedController;
//@property (nonatomic, strong) myViewController *viewController;
//直播session
@property (nonatomic, strong) AlivcLiveSession *liveSession;
//config配置类
@property (nonatomic, strong) AlivcLConfiguration *configuration;
@property (nonatomic, strong) AliVcMediaPlayer *player;
@property (nonatomic, strong) NSTimer *timer;
@property (nonatomic, strong) NSString *pushUrl;
@end
