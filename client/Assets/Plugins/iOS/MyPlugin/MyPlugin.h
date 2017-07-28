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
#import <AliyunPlayerSDK/AliyunPlayerSDK.h>
#import "UnityAppController.h"
#import "UI/UnityView.h"
#import "UI/UnityViewControllerBase.h"

@interface MyPlugin : UnityAppController <AlivcLiveSessionDelegate>;
//直播session
@property (nonatomic, strong) AlivcLiveSession *liveSession;
//config配置类
@property (nonatomic, strong) AlivcLConfiguration *configuration;
@property (nonatomic, strong) UITextView *myTextView;
@property (nonatomic, strong) AliVcMediaPlayer *player;
@property (nonatomic, strong) UIView *mShowView;
@property (nonatomic, strong) UIView *mPreView;
@property (nonatomic, strong) NSTimer *timer;
@property (nonatomic, strong) NSString *pushUrl;
@property (assign) CGPoint originPosition;
@end
