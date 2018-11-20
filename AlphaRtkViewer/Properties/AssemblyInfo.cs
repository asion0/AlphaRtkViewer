using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("RTK Viewer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("RtkViewer")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
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
[assembly: AssemblyVersion("1.0.0.9")]
[assembly: AssemblyFileVersion("1.0.0.9")]

//1.0.0.9 - 20181105, Modify earth view ele draw scale, request from Oliver.
//1.0.0.9 - 20181105, Support [Save Device Output], request from Oliver.
//1.0.0.9 - 20181030, Fixed issues in ming-jen's NB, add log for DebugView.
//1.0.0.8 - 20181026, Support ROM recovery and 3 update servers.
//1.0.0.7 - 20181022, Check file exist before delete.
//1.0.0.6 - 20181022, Add About, User Guide, fixed Convert crash issue.
//1.0.0.2 - 20181011, Add firmware update flow.
//1.0.0.1 - 20181004, First version for release
