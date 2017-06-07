//
//  myViewController.m
//  Unity-iPhone
//
//  Created by 朱賢譯 on 2017/6/6.
//
//
#import "myViewController.h"

@implementation myViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    NSLog(@"viewDidLoad()........");
    // Do any additional setup after loading the view.
    //viewController.view = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    //[viewController.view setBackgroundColor:[UIColor greenColor]];
    // 为了体现这个是自定义视图及控制器，随便在view上加点内容
    [self.view setBackgroundColor:[UIColor greenColor]];
    UILabel *label = [UILabel new];
    [label setText:@"this is container view"];
    [label setFrame:CGRectMake(100, 100, 0, 0)];
    [label sizeToFit];
    [self.view addSubview:label];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
