using BlazorBuilds.Components.TabControl;

namespace BlazorBuilds.Components.Common.Seeds;

internal class GlobalStrings
{
    internal const string Tab_Control_Class                       = "tab-control";
    internal const string Tabs_Class                              = "tab-control__tabs";
    internal const string Tabs_Wrap_Modifier_Class                = "tab-control__tabs--wrap";
    internal const string Tab_Class                               = "tab-control__tab";
    internal const string Tab_Active_Modifier_Class               = "tab-control__tab--active";
    internal const string Tab_Title_Class                         = "tab-control__tab-title";
    internal const string Tab_Panel_Content_Class                 = "tab-control__content";
    internal const string Tab_Panel_Content_Hidden_Modifier_Class = "tab-control__content--hidden";
             
    internal const string Tab_Focus_Colour_Css_Var                = "--tab-focus-clr";
    internal const string Tab_And_Panel_Background_Color_Css_Var  = "--tab-control-bg-clr";
            
    internal const string KeyBoard_Left_Arrow_Key                 = "ArrowLeft";
    internal const string KeyBoard_Right_Arrow_Key                = "ArrowRight";
    internal const string KeyBoard_Home_Key                       = "Home";
    internal const string KeyBoard_End_Key                        = "End";
    internal const string KeyBoard_Tab_Key                        = "Tab";

    internal const string Error_Message_Needs_Tab_Control         = "TabPages can only exist within a valid TabControl component";
    internal const string Error_Message_Needs_Aria_Tab_Name       = "The Aria Tab Name cannot be null, empty or whitespace";

}
