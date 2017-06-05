//
//  MyPlugin.h
//  MyPlugin
//
//  Created by 朱賢譯 on 2017/6/5.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AlivcLiveVideo/AlivcLiveVideo.h>

@interface MyPlugin : UIViewController
@property (nonatomic, strong) AlivcLiveSession *liveSession;
@property (nonatomic, strong) AlivcLConfiguration *configuration;
@property (nonatomic, strong) NSString *pushUrl;
@end
