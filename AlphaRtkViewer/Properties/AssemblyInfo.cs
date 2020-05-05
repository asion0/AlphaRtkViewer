using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("RTK Viewer+")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("RtkViewer+")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設定為 false 會使得這個組件中的類型
// 對 COM 元件而言為不可見。如果您需要從 COM 存取這個組件中
// 的類型，請在該類型上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

// 下列 GUID 為專案公開 (Expose) 至 COM 時所要使用的 typelib ID
[assembly: Guid("f4518967-b561-4825-9c02-fc3b2537220f")]

// 組件的版本資訊由下列四個值所組成: 
//
//      主要版本
//      次要版本 
//      組建編號
//      修訂編號
//
// 您可以指定所有的值，也可以依照以下的方式，使用 '*' 將組建和修訂編號
// 指定為預設值: 
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.6")]
[assembly: AssemblyFileVersion("2.0.0.6")]

//2.0.0.6 - 20200504, Support Viewer mode, request from Jim Lin
//2.0.0.5 - 20200410, Adjust Binsize timeout from 3000ms to 8000ms, request from user
//2.0.0.4 - 20200401, Display 4 system support, request from Oliver
//2.0.0.3 - 20200309, Display Galileo support, request from Oliver
//2.0.0.2 - 20200213, Modify Phoenix download flow for ROM Mode, request from customer
//2.0.0.1 - 20200212, Modify for Phoenix Alpha+, request from Oliver
//1.0.0.19 - 20200120, Modify for Phoenix, request from Oliver
//1.0.0.18 - 20190911, Modify fix mode in RTCM output, request from Jim Lin
//1.0.0.17 - 20190411, no leap seconds in parsing time stamp for PPK application, report by Jim Lin
//1.0.0.16 - 20190329, fixed parsing time stamp issue, report by Jim Lin
//1.0.0.15 - 20190318, Add RTCM parser and Tool for parsing time stamp raw, request from Jim Lin
//1.0.0.14 - 20190215, Add an exception handler for COM port string conversion, report by Giorgios https://forum.polaris-gnss.com/t/rtkview-issue/217
//1.0.0.13 - 20190117, Allow multiple instances, request from Oliver
//1.0.0.12 - 20190111, Fix UBLOX message index error, report by Ming-Jen.
//1.0.0.11 - 20181217, Fix issue in Get COM Port List, report by Oliver's mail.
//1.0.0.10 - 20181207, Fix issue in Configure RTK Mode, report by user mhmagnuson.
//1.0.0.9 - 20181105, Modify earth view ele draw scale, request from Oliver.
//1.0.0.9 - 20181105, Support [Save Device Output], request from Oliver.
//1.0.0.9 - 20181030, Fixed issues in ming-jen's NB, add log for DebugView.
//1.0.0.8 - 20181026, Support ROM recovery and 3 update servers.
//1.0.0.7 - 20181022, Check file exist before delete.
//1.0.0.6 - 20181022, Add About, User Guide, fixed Convert crash issue.
//1.0.0.2 - 20181011, Add firmware update flow.
//1.0.0.1 - 20181004, First version for release
