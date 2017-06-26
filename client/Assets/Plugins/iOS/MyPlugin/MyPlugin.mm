//
//  MyPlugin.m
//  MyPlugin
//
//  Created by 朱賢譯 on 2017/6/5.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import "MyPlugin.h"

@implementation MyPlugin

static MyPlugin *_sharedInstance = nil;
static dispatch_once_t _onceToken;

+ (MyPlugin *)sharedInstance {
    dispatch_once(&_onceToken, ^{
        _sharedInstance = [[MyPlugin alloc] init];
        
    });
    return _sharedInstance;
}

- (void)willStartWithViewController:(UIViewController*)controller {
    NSLog(@"willStartWithViewController()");
    //NSLog(@"%", controller);
    // 新建自定义视图控制器。
    //self.viewController = [[myViewController alloc] init];
    UIViewController *viewController = [[UIViewController alloc] init];
    //屏幕尺寸
    //CGFloat ApplicationW = [[UIScreen mainScreen] bounds].size.width;
    //CGFloat ApplicationH = [[UIScreen mainScreen] bounds].size.height;
    //self.mShowView = [[UIView alloc] initWithFrame:CGRectMake(300, 0, 500, 500)];
    viewController.view = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    //[viewController.view setBackgroundColor:[UIColor greenColor]];
    //if(_unityView) {
    //[viewController.view setFrame:CGRectMake(0, 0, 500, 500)];
        // 把Unity的内容视图作为子视图放到我们自定义的视图里面。
    [viewController.view addSubview:_unityView];
    
        // 把Unity的内容视图作为子视图放到我们自定义的视图里面。
        //[self.viewController.view addSubview:_unityView];
        //[_unityView setFrame:CGRectMake(0, 0, ApplicationW/3*2, ApplicationH)];
        //_unityView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    //}
    // 把根视图和控制器全部换成我们自定义的内容。
    _rootController = viewController;
    _rootView = _rootController.view;
    
    self.myTextView = [[UITextView alloc] initWithFrame:CGRectMake(300, 0, 300, 150)];
    self.myTextView.text = @"...";
    self.myTextView.backgroundColor = [UIColor whiteColor];
    self.myTextView.layer.opacity = 0.5;
    //some other setup like setting the font for the UITextView...
    [_rootView addSubview:self.myTextView];
    
    UIButton *button1 = [UIButton buttonWithType:UIButtonTypeRoundedRect];
    button1.frame = CGRectMake(20, 20, 60, 60);
    [button1 setTitle:@"PushS" forState:(UIControlState)UIControlStateNormal];
    //[button1 setTitleColor:[UIColor blackColor] forState:UIControlStateNormal];
    button1.backgroundColor = [UIColor greenColor];
    button1.layer.borderColor = [UIColor blackColor].CGColor;
    button1.layer.borderWidth = 1.0f;
    button1.layer.cornerRadius = 10.0f;
    [button1 addTarget:self action:@selector(goToFirstTrailer1)
      forControlEvents:(UIControlEvents)UIControlEventTouchDown];
    [_rootView addSubview:button1];
    
    UIButton *button2 = [UIButton buttonWithType:UIButtonTypeRoundedRect];
    button2.frame = CGRectMake(100, 20, 60, 60);
    [button2 setTitle:@"PushE" forState:(UIControlState)UIControlStateNormal];
    button2.backgroundColor = [UIColor greenColor];
    button2.layer.borderColor = [UIColor blackColor].CGColor;
    button2.layer.borderWidth = 1.0f;
    button2.layer.cornerRadius = 10.0f;
    [button2 addTarget:self action:@selector(goToFirstTrailer2)
      forControlEvents:(UIControlEvents)UIControlEventTouchDown];
    [_rootView addSubview:button2];
    
    UIButton *button3 = [UIButton buttonWithType:UIButtonTypeRoundedRect];
    button3.frame = CGRectMake(20, 100, 60, 60);
    [button3 setTitle:@"PlayS" forState:(UIControlState)UIControlStateNormal];
    button3.backgroundColor = [UIColor blueColor];
    button3.layer.borderColor = [UIColor blackColor].CGColor;
    button3.layer.borderWidth = 1.0f;
    button3.layer.cornerRadius = 10.0f;
    [button3 addTarget:self action:@selector(goToFirstTrailer3)
      forControlEvents:(UIControlEvents)UIControlEventTouchDown];
    [_rootView addSubview:button3];
    
    UIButton *button4 = [UIButton buttonWithType:UIButtonTypeRoundedRect];
    button4.frame = CGRectMake(100, 100, 60, 60);
    [button4 setTitle:@"PlayE" forState:(UIControlState)UIControlStateNormal];
    button4.backgroundColor = [UIColor blueColor];
    button4.layer.borderColor = [UIColor blackColor].CGColor;
    button4.layer.borderWidth = 1.0f;
    button4.layer.cornerRadius = 10.0f;
    [button4 addTarget:self action:@selector(goToFirstTrailer4)
      forControlEvents:(UIControlEvents)UIControlEventTouchDown];
    [_rootView addSubview:button4];
    
    
    CGFloat ApplicationW = [[UIScreen mainScreen] bounds].size.width;
    CGFloat ApplicationH = [[UIScreen mainScreen] bounds].size.height;
    
    //self.mShowView = [[UIView alloc] init];
    self.mShowView = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    self.mShowView.transform = CGAffineTransformMakeRotation(M_PI);
    self.mShowView.layer.anchorPoint = CGPointMake(1.5, 0.5);
    [self.mShowView setBounds: CGRectMake(0, 0, ApplicationW/3, ApplicationH)];
    [self.mShowView setBackgroundColor:[UIColor greenColor]];
    [_rootView insertSubview: self.mShowView atIndex: -1];
    //[_rootView addSubview:self.mShowView];
    
    //[self initPlayer];
    
    dispatch_once(&_onceToken, ^{
        _sharedInstance = self;
    });
    _timer = [NSTimer scheduledTimerWithTimeInterval:1.0 target:self selector:@selector(timeUpdate) userInfo:nil repeats:YES];
}

-(void)goToFirstTrailer1 {
    NSLog(@"goToFirstTrailer1()");
    //[self startAnimator:@"OutsideToTrailer1_" forNumFrames:60];
    //[self initRecord];
    [self startRecord];
}

-(void)goToFirstTrailer2 {
    NSLog(@"goToFirstTrailer2()");
    //[self startAnimator:@"OutsideToTrailer1_" forNumFrames:60];
    //NSString *arg = @"rtmp://192.168.20.178:1935/mj17/myStream";
    [self stopRecord];
}

-(void)goToFirstTrailer3 {
    NSLog(@"goToFirstTrailer3()");
    //[self startAnimator:@"OutsideToTrailer1_" forNumFrames:60];
    NSString *arg = @"rtmp://192.168.20.178:1935/mj17/myStream";
    [self startPlay:arg];
}

-(void)goToFirstTrailer4 {
    NSLog(@"goToFirstTrailer4()");
    //[self startAnimator:@"OutsideToTrailer1_" forNumFrames:60];
    //NSString *arg = @"rtmp://192.168.20.178:1935/mj17/myStream";
    [self stopPlay];
}

/*!
 * 推流错误
 */
- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session error:(NSError *)error{
    NSString *msg = [NSString stringWithFormat:@"%zd %@",error.code, error.localizedDescription];
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        NSString *msg = [NSString stringWithFormat:@"%zd %@",error.code, error.localizedDescription];
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Live Error" message:msg delegate:nil cancelButtonTitle:@"取消" otherButtonTitles:@"重新连接", nil];
        alertView.delegate = self;
        [alertView show];
    });
    */
    NSLog(@"!!!error : %@", msg);
}

- (void)alivcLiveVideoReconnectTimeout:(AlivcLiveSession*)session {
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"提示" message:@"重连超时（此处根据实际情况决定，默认重连时长5s，可更改，建议开发者在此处重连）" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles: nil];
        
        [alertView show];
    });
     */
    NSString *msg = @"重连超时（此处根据实际情况决定，默认重连时长5s，可更改，建议开发者在此处重连）";
    NSLog(@"msg=%@", msg);
}


- (void)alivcLiveVideoLiveSessionConnectSuccess:(AlivcLiveSession *)session {
    //dispatch_async(dispatch_get_main_queue(), ^{
    NSLog(@"connect success! session=%@", session);
    //});
    //NSString *arg = @"rtmp://192.168.20.5:1935/17MJ/myStream";
     //self.pushUrl = @"rtmp://192.168.20.178:1935/mj17/myStream";
    //[_sharedInstance startPlay: arg];
}


- (void)alivcLiveVideoLiveSessionNetworkSlow:(AlivcLiveSession *)session{
    // 注意：一定要套 主线程 完成UI操作
    //dispatch_async(dispatch_get_main_queue(), ^{
        //self.textView.text = @"网速太慢";
    //});
    dispatch_async(dispatch_get_main_queue(), ^{
        self.myTextView.text = @"网络很慢，已经不建议直播";
        NSLog(@"网络很慢，已经不建议直播");
    });
}

- (void)alivcLiveVideoOpenAudioSuccess:(AlivcLiveSession *)session {
    //dispatch_async(dispatch_get_main_queue(), ^{
    //    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"YES" message:@"麦克风打开成功" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
    //    [alertView show];
    //});
    dispatch_async(dispatch_get_main_queue(), ^{
        NSLog(@"麦克风打开成功");
    });
}

- (void)alivcLiveVideoOpenVideoSuccess:(AlivcLiveSession *)session {
    //dispatch_async(dispatch_get_main_queue(), ^{
    //    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"YES" message:@"摄像头打开成功" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
    //    [alertView show];
    //});
    dispatch_async(dispatch_get_main_queue(), ^{
        NSLog(@"摄像头打开成功");
    });
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session openAudioError:(NSError *)error {
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"麦克风获取失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    */
    NSLog(@"麦克风获取失败");
    
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session openVideoError:(NSError *)error {
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"摄像头获取失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    */
    NSLog(@"摄像头获取失败");
    
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session encodeAudioError:(NSError *)error {
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"音频编码初始化失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    */
    NSLog(@"音频编码初始化失败");
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session encodeVideoError:(NSError *)error {
    /*
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"视频编码初始化失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
     */
    NSLog(@"视频编码初始化失败");
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session bitrateStatusChange:(ALIVC_LIVE_BITRATE_STATUS)bitrateStatus {
    
    //dispatch_async(dispatch_get_main_queue(), ^{
    NSLog(@"升降码率:%ld", bitrateStatus);
    //});
}

- (void)timeUpdate {
    AlivcLDebugInfo *i = [self.liveSession dumpDebugInfo];
    NSDate *date = [NSDate dateWithTimeIntervalSince1970:i.connectStatusChangeTime];
    
    NSMutableString *msg = [[NSMutableString alloc] init];
    [msg appendFormat:@"CycleDelay(%0.2fms)\n",i.cycleDelay];
    [msg appendFormat:@"bitrate(%zd) buffercount(%zd)\n",[self.liveSession alivcLiveVideoBitRate] ,self.liveSession.dumpDebugInfo.localBufferVideoCount];
    [msg appendFormat:@" efc(%zd) pfc(%zd)\n",i.encodeFrameCount, i.pushFrameCount];
    [msg appendFormat:@"%0.2ffps %0.2fKB/s %0.2fKB/s\n", i.fps,i.encodeSpeed, i.speed/1024];
    [msg appendFormat:@"%lluB pushSize(%lluB) status(%zd) %@",i.localBufferSize, i.pushSize, i.connectStatus, date];
    [msg appendFormat:@" %0.2fms\n",i.localDelay];
    [msg appendFormat:@"video_pts:%zd\naudio_pts:%zd\n", i.currentVideoPTS,i.currentAudioPTS];
    [msg appendFormat:@"fps:%f\n", i.fps];
    //NSLog(msg);
    //textView.text = msg;
    self.myTextView.text = msg;
}


// 设置直播参数
- (void)setupConfiguration {
    NSLog(@"setupConfiguration()...");
    //1.初始化config配置类
    self.configuration = [[AlivcLConfiguration alloc] init];
    //2. 设置推流地址
    self.pushUrl = @"rtmp://192.168.20.178:1935/mj17/myStream";
    self.configuration.url = self.pushUrl;
    NSLog(@"pushurl=%@", self.configuration.url);
    //3. 设置最大码率
        self.configuration.videoMaxBitRate = 1500 * 1000;
    //4. 设置当前视频码率

    self.configuration.videoBitRate = 600 * 1000;
    //5. 设置最小码率

    self.configuration.videoMinBitRate = 400 * 1000;
    //6. 设置音频码率

    self.configuration.audioBitRate = 64 * 1000;
    //7. 设置直播分辨率
    //self.configuration.videoSize = CGSizeMake(360, 640);
    self.configuration.videoSize = CGSizeMake(640, 360);
    //8. 设置横屏or竖屏 默认竖屏
    self.configuration.screenOrientation = AlivcLiveScreenHorizontal;
    //self.configuration.screenOrientation = AlivcLiveScreenVertical;
    //9. 设置帧率 default 20
    self.configuration.fps = 20;
    //10. 设置摄像头采集质量
    self.configuration.preset = AVCaptureSessionPresetiFrame1280x720;
    //11. 设置前置摄像头或后置摄像头
    self.configuration.position = AVCaptureDevicePositionFront;
    //12.设置水印图片 默认无水印
    //self.configuration.waterMaskImage = [UIImage imageNamed:@"watermask"];
    //13.设置水印位置
    //self.configuration.waterMaskLocation = 1;
    //14.设置水印相对x边框距离
    //self.configuration.waterMaskMarginX = 10;
    //15.设置水印相对y边框距离
    //self.configuration.waterMaskMarginY = 10;
    //16.设置重连超时时长
    self.configuration.reconnectTimeout = 5;
}

- (void)setupLiveSession {
    NSLog(@"setupLiveSession()");
    //1. 初始化liveSession类
    self.liveSession = [[AlivcLiveSession alloc] initWithConfiguration:self.configuration];
    //[self.liveSession alivcLiveVideoRotateCamera];
    //2. 设置session代理
    self.liveSession.delegate = self;
    //3. 开启直播预览
    [self.liveSession alivcLiveVideoStartPreview];
    
    //[self.liveSession alivcLiveVideoRotateCamera];
    
    //self.liveSession.previewView.transform = CGAffineTransformMakeRotation(M_PI);
    /*
    [self.liveSession alivcLiveVideoUpdateConfiguration:^(AlivcLConfiguration *configuration) {
        configuration.videoMaxBitRate = 1500 * 1000;
        configuration.videoBitRate = 600 * 1000;
        configuration.videoMinBitRate = 400 * 1000;
        configuration.audioBitRate = 64 * 1000;
        configuration.fps = 20;
    }];
     */
 
    
    //[self.liveSession previewView]
    //4. 推流连接
    [self.liveSession alivcLiveVideoConnectServer];
    
    CGFloat ApplicationW = [[UIScreen mainScreen] bounds].size.width;
    CGFloat ApplicationH = [[UIScreen mainScreen] bounds].size.height;
    NSLog(@"ApplicationW=%f, ApplicationH=%f", ApplicationW, ApplicationH);
    
    [self.liveSession.previewView setBounds: CGRectMake(0, 0, ApplicationH, ApplicationW/3)];
    //self.liveSession.previewView.autoresizesSubviews = YES;
    self.liveSession.previewView.transform = CGAffineTransformMakeRotation(M_PI/2);
    self.liveSession.previewView.layer.anchorPoint = CGPointMake(0.5, 1.5);
    
    //self.liveSession.previewView.layer.cornerRadius = self.liveSession.previewView.bounds.size.height /2;
    self.liveSession.previewView.layer.masksToBounds = YES;
    self.liveSession.previewView.layer.borderWidth = 1;
    //self.liveSession.previewView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    //[self.view addSubview:[self.liveSession previewView]];
    
    //开启美颜
    [self.liveSession setEnableSkin:YES];
    //缩放
    [self.liveSession alivcLiveVideoZoomCamera:1.0f];
    //聚焦
    //[self.liveSession alivcLiveVideoFocusAtAdjustedPoint:percentPoint autoFocus:YES];
    //调试信息
    //AlivcLDebugInfo  *i = [self.liveSession dumpDebugInfo];
    //静音
    //[self.liveSession setMute:YES];
    //[self.liveSession previewView];
    
    //5. 非常重要
    dispatch_async(dispatch_get_main_queue(), ^{
        //[_rootView addSubview: [self.liveSession previewView]];
        [_rootView insertSubview:[self.liveSession previewView] atIndex:0];
    });
    
    //dispatch_async(dispatch_get_main_queue(), ^{
    //    //    //[self.view insertSubview:[self.liveSession previewView] atIndex:0];
    //    //    [self.view addSubview:[self.liveSession previewView]];
    //    [_rootView addSubview: [self.liveSession previewView]];
    //});
    
}


-(void)initRecord{
    NSLog(@"initRecord()....%@", self);
    //[UnityGetGLViewController() presentViewController: animated: completion:nil];
    [self setupConfiguration];
}

-(void)startRecord{
    NSLog(@"startRecord()....%@", self);
    if(self.liveSession==nil)
        [self initRecord];
    [self setupLiveSession];
}

-(void)stopRecord{
    //停止预览，注意:停止预览后将liveSession置为nil
    [self.liveSession alivcLiveVideoStopPreview];
    [self.liveSession.previewView removeFromSuperview];
    //关闭直播
    [self.liveSession alivcLiveVideoDisconnectServer];
    //销毁直播 session
    self.liveSession = nil;
}

-(void)initPlayer{
    NSLog(@"initPlayer()....%@", self);
    //初始化播放器的类
    self.player = [[AliVcMediaPlayer alloc] init];
    //创建播放器，传入显示窗口
    [self.player create:self.mShowView];
    //注册准备完成通知
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(OnVideoPrepared:) name:AliVcMediaPlayerLoadDidPreparedNotification object:self.player];
    //注册错误通知
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(OnVideoError:) name:AliVcMediaPlayerPlaybackErrorNotification object:self.player];
}

-(void)startPlay: (NSString *)url
{
    if(self.player==nil)
        [self initPlayer];
    NSLog(@"startPlay()....%@", url);
    //传入播放地址，准备播放
    //NSURL *mUrl = [[NSURL alloc] initWithString:url];
    NSURL * mUrl = [NSURL URLWithString:url];
    //NSURL *mUrl = [[NSURL alloc] initWithString:url];
    [self.player prepareToPlay:mUrl];
    
    [_rootView bringSubviewToFront:self.mShowView];
    //开始播放
    [self.player play];
    
}

-(void)stopPlay{
    NSLog(@"stopPlay()....%@", self);
    [self.player stop];
    [_rootView sendSubviewToBack: self.mShowView];
}


//recieve prepared notification
- (void)OnVideoPrepared:(NSNotification *)notification {
    
    //NSTimeInterval duration = self.player.duration;
    //playSlider.maximumValue = duration;
    //playSlider.value = self.player.currentPosition;
    NSLog(@"notification=%@", notification);
    //[self hideLoadingIndicators];
    //if(duration == 0){
        //        seekForwardButton.hidden = YES;
        //        seekBackwardButton.hidden = YES;
        //playBtn.hidden = YES;
        //playSlider.hidden = YES;
        //playTimeLabel.hidden = YES;
        //remainTimeLaber.hidden = YES;
    //}
}

- (void)OnVideoError:(NSNotification *)notification {
    
    NSString *error_msg = @"未知错误";
    AliVcMovieErrorCode error_code = self.player.errorCode;
    
    switch (error_code) {
        case ALIVC_ERR_FUNCTION_DENIED:
            error_msg = @"未授权";
            break;
        case ALIVC_ERR_ILLEGALSTATUS:
            error_msg = @"非法的播放流程";
            break;
        case ALIVC_ERR_INVALID_INPUTFILE:
            error_msg = @"无法打开";
            //[self hideLoadingIndicators];
            break;
        case ALIVC_ERR_NO_INPUTFILE:
            error_msg = @"无输入文件";
            //[self hideLoadingIndicators];
            break;
        case ALIVC_ERR_NO_NETWORK:
            error_msg = @"网络连接失败";
            break;
        case ALIVC_ERR_NO_SUPPORT_CODEC:
            error_msg = @"不支持的视频编码格式";
            //[self hideLoadingIndicators];
            break;
        case ALIVC_ERR_NO_VIEW:
            error_msg = @"无显示窗口";
            //[self hideLoadingIndicators];
            break;
        case ALIVC_ERR_NO_MEMORY:
            error_msg = @"内存不足";
            break;
        case ALIVC_ERR_DOWNLOAD_TIMEOUT:
            error_msg = @"网络超时";
            break;
        case ALIVC_ERR_UNKOWN:
            error_msg = @"未知错误";
            break;
        default:
            break;
    }
    
    //NSLog(error_msg);
    if(error_code == ALIVC_ERR_DOWNLOAD_TIMEOUT) {
        
        [self.player pause];
        
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"错误提示"
                                                        message:error_msg
                                                       delegate:self
                                              cancelButtonTitle:@"等待"
                                              otherButtonTitles:@"重新连接",nil];
        
        [alert show];
    }
    //the error message is important when error_cdoe > 500
    else if(error_code > 500 || error_code == ALIVC_ERR_FUNCTION_DENIED) {
        [self.player reset];
        //UIAlertView *alter = [[UIAlertView alloc] initWithTitle:[self.mSourceURL absoluteString] message:error_msg delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        //
        //[alter show];
     
        //return;
    }
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(MyPlugin)



// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
extern "C" {
    //-------------------------------------------------------------------------------------------------
    void _initRecord(){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj initRecord];
    }
    void _stopRecord (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj stopRecord];
    }
    void _startRecord (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj startRecord];
    }
    void _stopPlay (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj stopPlay];
    }
    void _startPlay (const char *str){
        MyPlugin *obj = [MyPlugin sharedInstance];
        NSString *arg = [NSString stringWithUTF8String: str];
        //NSURL *url = [[NSURL alloc] initWithString: arg];
        [obj startPlay : arg];
    }
}
