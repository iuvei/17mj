//
//  MyPlugin.m
//  MyPlugin
//
//  Created by 朱賢譯 on 2017/6/5.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import "MyPlugin.h"

@implementation MyPlugin

+ (MyPlugin *)GetSharedInstance {
    static MyPlugin *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyPlugin alloc] init];
        
    });
    return sharedInstance;
}

/*!
 * 推流错误
 */
- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session error:(NSError *)error {
    
}

/*!
 * 网络很慢，已经不建议直播
 */
- (void)alivcLiveVideoLiveSessionNetworkSlow:(AlivcLiveSession *)session {
    
    
}

-(void)initRecord{
    self.pushUrl = @"192.168.20.178/mj17/myStream";
    /*
    NSError *goCoderLicensingError = [WowzaGoCoder registerLicenseKey:@"GOSK-B543-0103-39FC-DA5D-0BD0"];
    if (goCoderLicensingError != nil) {
        // Log license key registration failure
        NSLog(@"%@", [goCoderLicensingError localizedDescription]);
    } else {
        // Initialize the GoCoder SDK
        self.goCoder = [WowzaGoCoder sharedInstance];
    }
    
    if (self.goCoder != nil) {
        // Associate the U/I view with the SDK camera preview
        CGRect viewRect = CGRectMake(200, 0, 300, 190);
        self.unityView = UnityGetGLView();
        self.videoView = [[UIView alloc] initWithFrame:viewRect];
        self.goCoder.cameraView = self.videoView;
        
        // Set the active camera to the front camera if it is not already active
        if (self.goCoder.cameraPreview.camera.direction != WZCameraDirectionFront)
            [self.goCoder.cameraPreview switchCamera];
        
        // Start the camera preview
        [self.goCoder.cameraPreview startPreview];
    }
    
    // Get a copy of the active config
    WowzaConfig *goCoderBroadcastConfig = self.goCoder.config;
    
    // Set the defaults for 720p video
    [goCoderBroadcastConfig loadPreset:WZFrameSizePreset352x288];
    
    // Set the connection properties for the target Wowza Streaming Engine server or Wowza Cloud account
    goCoderBroadcastConfig.hostAddress = @"192.168.20.171";
    goCoderBroadcastConfig.portNumber = 1935;
    goCoderBroadcastConfig.applicationName = @"17MJ";
    goCoderBroadcastConfig.streamName = @"myStream";
    //goCoderBroadcastConfig.username = @"orson";
    //goCoderBroadcastConfig.password = @"orson168";
    // Update the active config
    self.goCoder.config = goCoderBroadcastConfig;
    //[self.broadcastButton addTarget:self action:@selector(broadcastButtonTapped:)
    //               forControlEvents:UIControlEventTouchUpInside];
    */
    
    //1.初始化config配置类
    self.configuration = [[AlivcLConfiguration alloc]init];
    //2. 设置推流地址
    self.configuration.url = self.pushUrl;
    //3. 设置最大码率
    /*!
     *  最大码率，网速变化的时候会根据这个值来提供建议码率
     *  默认 1500 * 1000
     */
    self.configuration.videoMaxBitRate = 1500 * 1000;
    //4. 设置当前视频码率
    /*!
     *  默认码率，在最大码率和最小码率之间
     *  默认 600 * 1000
     */
    self.configuration.videoBitRate = 600 * 1000;
    //5. 设置最小码率
    /*!
     *  默认码率，在最大码率和最小码率之间
     *  默认 600 * 1000
     */
    self.configuration.videoMinBitRate = 400 * 1000;
    //6. 设置音频码率
    /*!
     *  音频码率
     *  默认 64 * 1000
     */
    self.configuration.audioBitRate = 64 * 1000;
    //7. 设置直播分辨率
    self.configuration.videoSize = CGSizeMake(360, 640);
    //8. 设置横屏or竖屏 默认竖屏
    self.configuration.screenOrientation = AlivcLiveScreenVertical;
    //9. 设置帧率 default 20
    self.configuration.fps = 20;
    //10. 设置摄像头采集质量
    self.configuration.preset = AVCaptureSessionPresetiFrame1280x720;
    //11. 设置前置摄像头或后置摄像头
    self.configuration.position = AVCaptureDevicePositionBack;
    //12.设置水印图片 默认无水印
    self.configuration.waterMaskImage = [UIImage imageNamed:@"watermask"];
    //13.设置水印位置
    self.configuration.waterMaskLocation = 1;
    //14.设置水印相对x边框距离
    self.configuration.waterMaskMarginX = 10;
    //15.设置水印相对y边框距离
    self.configuration.waterMaskMarginY = 10;
    //16.设置重连超时时长
    self.configuration.reconnectTimeout = 5;
    
}

-(void)startRecord{
    //1. 初始化liveSession类
    self.liveSession = [[AlivcLiveSession alloc]initWithConfiguration:self.configuration];
    //2. 设置session代理
    self.liveSession.delegate = self;
    //3. 开启直播预览
    [self.liveSession alivcLiveVideoStartPreview];
    //4. 推流连接
    [self.liveSession alivcLiveVideoConnectServer];
    
    //5. 非常重要
    dispatch_async(dispatch_get_main_queue(), ^{
        [self.view insertSubview:[self.liveSession previewView] atIndex:0];
    });
    
    /*
    // Ensure the minimum set of configuration settings have been specified necessary to
    // initiate a broadcast streaming session
    NSError *configValidationError = [self.goCoder.config validateForBroadcast];
    
    if (configValidationError != nil) {
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Incomplete Streaming Settings"
                                   message: self.goCoder.status.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    } else if (self.goCoder.status.state != WZStateRunning) {
        // Start streaming
        [self.goCoder startStreaming:self];
        //[self.unityView addSubview:self.videoView];
        [self.unityView.superview addSubview:self.videoView];
        //NSString *str =[[NSString alloc] init];
        //NSString *url = [[NSString alloc] init];
        NSString *url = @"rtsp://192.168.20.238:1935/17MJ";
        //self.goCoder.config.hostAddress
        UnitySendMessage("GameManager", "OnRecordStart", url.UTF8String);
        //self.unityView.layer.zPosition = 1;<#(nonnull UIView *)#>
        //[self.unityView.superview insertSubview:self.videoView atIndex:1];
        //NSLog(@"self.unityView.superviews %@", self.unityView.superview.subviews);
    }
     */
}
-(void)stopRecord{
    //停止预览，注意:停止预览后将liveSession置为nil
    [self.liveSession alivcLiveVideoStopPreview];
    //关闭直播
    [self.liveSession alivcLiveVideoDisconnectServer];
    //销毁直播 session
    self.liveSession = nil;
    /*
    // Ensure the minimum set of configuration settings have been specified necessary to
    // initiate a broadcast streaming session
    NSError *configValidationError = [self.goCoder.config validateForBroadcast];
    
    if (configValidationError != nil) {
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Incomplete Streaming Settings"
                                   message: self.goCoder.status.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    } else if (self.goCoder.status.state == WZStateRunning) {
        // Stop the broadcast that is currently running
        [self.goCoder endStreaming:self];
        [self.videoView removeFromSuperview];
        //[parent remove
    }
     */
}
/*
- (void) onWZStatus:(WZStatus *) goCoderStatus {
    // A successful status transition has been reported by the GoCoder SDK
    NSString *statusMessage = nil;
    
    switch (goCoderStatus.state) {
        case WZStateIdle:
            statusMessage = @"The broadcast is stopped";
            break;
            
        case WZStateStarting:
            statusMessage = @"Broadcast initialization";
            break;
            
        case WZStateRunning:
            statusMessage = @"Streaming is active";
            break;
            
        case WZStateStopping:
            statusMessage = @"Broadcast shutting down";
            break;
    }
    
    if (statusMessage != nil)
        NSLog(@"Broadcast status: %@", statusMessage);
}

- (void) onWZError:(WZStatus *) goCoderStatus {
    // If an error is reported by the GoCoder SDK, display an alert dialog
    // containing the error details using the U/I thread
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Streaming Error"
                                   message:goCoderStatus.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    });
}
*/

@end



// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
#if __cplusplus
extern "C" {
#endif
    //-------------------------------------------------------------------------------------------------
    void _initRecord(){
        MyPlugin *objVideoCapture = [MyPlugin GetSharedInstance];
        [objVideoCapture initRecord];
    }
    //-------------------------------------------------------------------------------------------------
    //void _tapRecord (){
    //    MyPlugin *objVideoCapture = [MyPlugin GetSharedInstance];
    //    [objVideoCapture tapRecord];
    //}
    void _stopRecord (){
        MyPlugin *objVideoCapture = [MyPlugin GetSharedInstance];
        [objVideoCapture stopRecord];
    }
    void _startRecord (){
        MyPlugin *objVideoCapture = [MyPlugin GetSharedInstance];
        [objVideoCapture startRecord];
    }
#if __cplusplus
}
#endif

