using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("DC.Communication.Components")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DC.Communication.Components")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("31d59e5c-8aa0-4326-b1ba-b978bff1fbfd")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.13")]
[assembly: AssemblyFileVersion("1.0.0.13")]


//*****************版本说明*******************
//v1.0.0.2 20140930 此版本为完成C2000模块的所有功能，并应用到公安视频电源箱上
//v1.0.0.3 20161020 此版本主新增对老c2000模块及新网桥类型的识别
//v1.0.0.4 20161220 此版本主要修改
//                  1.修改UDP查询设备时，MAC地址获取来源位数的修改；
//                  2.屏蔽Tcp中每秒自动检测网络连接状态的设置
//v1.0.0.5 20170313 新增UDP搜索需要的mkdz  jhjmac 两个字段的定义及解析；
//v1.0.0.6 20170605 重构UDP相关返回的对象和属性
//v1.0.0.7 20170622 主要对UDP搜索模块按新协议进行解析
//v1.0.0.8 20170630 修复设置网络参数的BUG
//v1.0.0.9 20170713 优化发送复位UDP包时，按指定IP发送（多网卡情况下，复位需要用 设置的服务器IP下发才会成功）
//v1.0.0.10 20170720 修改搜索网络模块DeviceType字段的解析  
//v1.0.0.11 20170815 优化TCP数据收发模块（增加数据收发时的入队和出队锁，解决高并发情况下，发送数据有时从队列获取不到数据的问题）
//v1.0.0.12 20171110 增加8962的TCP日志输出功能 
//v1.0.0.13 20180104 对接SocketTCPServer的新增和断开连接做优化处理，解决特定网络情况下内存泄露问题；

