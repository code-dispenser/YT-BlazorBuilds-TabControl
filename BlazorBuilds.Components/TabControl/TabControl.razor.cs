using BlazorBuilds.Components.Common.Extensions;
using BlazorBuilds.Components.Common.Seeds;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBuilds.Components.TabControl;

public partial class TabControl
{
    internal enum StyleInfo : int {TabControlClasses, TabClasses,TabsClasses,TabTitleClasses, BackgroundStyle, FocusStyle }
    
    private Dictionary<int, ElementReference> _tabButtonRefs = [];

    private List<TabPage>   _tabPages       = [];
    private int             _activeTabIndex = -1; //gets set and syncs with ActiveTabIndex in RaiseTabChanged
    private int             _focusIndex     = 0;

    [Parameter] public RenderFragment     ChildContent          { get; set; } = default!;
    [Parameter] public EventCallback<int> ActiveTabIndexChanged { get; set; }
    [Parameter] public int                ActiveTabIndex        { get; set; } = 0; 
    [Parameter] public string             TabFocusColour        { get; set; } = String.Empty;
    [Parameter] public string             TabsAndPanelBgColour  { get; set; } = String.Empty;
    [Parameter] public bool               WrapTabs              { get; set; } = false;
    [Parameter] public bool               AutoActivatePanel     { get; set; } = false;

    internal TabPage? ActiveTab { get; private set; } = null;

    public void AddTabPage(TabPage tabPage)

        => _tabPages.Contains(tabPage).WhenFalse(() => _tabPages.Add(tabPage));

    public void RemoveTabPage(TabPage tabPage)

        => _tabPages.Remove(tabPage);

    private async Task CheckRaiseTabChanged(int tabIndex)//called from the tab button onFocus event

        => await AutoActivatePanel.WhenTrue(() => RaiseTabChanged(tabIndex));

    private async Task RaiseTabChanged(int tabIndex)
    {
        tabIndex = tabIndex switch //just cycle if high or low index;
        {
            _ when tabIndex >= _tabPages.Count => 0,
            _ when tabIndex < 0 => _tabPages.Count - 1,
            _ => tabIndex
        };

        if (_activeTabIndex != tabIndex)//Only raise when changed
        {
            _activeTabIndex = _focusIndex  = tabIndex;

            if (_tabPages.Count > 0)
            {
                ActiveTab = _tabPages[tabIndex];

                await ActiveTabIndexChanged.HasDelegate.WhenTrue(() => ActiveTabIndexChanged.InvokeAsync(tabIndex));
            }
        }
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if (_tabPages.Count ==  0) return;//Tab pages not loaded yet, handle in OnAfterRenderAsync  

        await (ActiveTabIndex != _activeTabIndex).WhenTrue(() => RaiseTabChanged(ActiveTabIndex));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == true)
        {
            await firstRender.WhenTrue(() => RaiseTabChanged(ActiveTabIndex));
            /*
                * Check only needed on first render in case user is not using any tabindex event bindings
            */
            ActiveTabIndexChanged.HasDelegate.WhenFalse(() => StateHasChanged());
        }

    }

    private async Task HandleKeyPress(KeyboardEventArgs keyArgs)
    {
        int tabCount = _tabPages.Count -1;
        var focusToIndex = -1;

        focusToIndex = keyArgs.Key switch
        {
            GlobalStrings.KeyBoard_Left_Arrow_Key => (_focusIndex > 0 ? _focusIndex -1 : tabCount),
            GlobalStrings.KeyBoard_Right_Arrow_Key => (_focusIndex < tabCount ? _focusIndex + 1 : 0),
            GlobalStrings.KeyBoard_Home_Key => 0,
            GlobalStrings.KeyBoard_End_Key => tabCount,
            _ => -1
        };

        if (focusToIndex > -1 && focusToIndex != _focusIndex)
        {
            await _tabButtonRefs[focusToIndex].FocusAsync();
            _focusIndex = focusToIndex;
        }

        if (focusToIndex == -1 && keyArgs.Key == GlobalStrings.KeyBoard_Tab_Key) _focusIndex = _activeTabIndex;

    }

    private string GetStyleInfo(StyleInfo styleInfo, bool active = false)

        => styleInfo switch
        {
            StyleInfo.FocusStyle => (String.IsNullOrWhiteSpace(TabFocusColour) ? String.Empty : $"{GlobalStrings.Tab_Focus_Colour_Css_Var}:{TabFocusColour};").Replace(";;", ";"),
            StyleInfo.BackgroundStyle => (String.IsNullOrWhiteSpace(TabsAndPanelBgColour) ? String.Empty : $"{GlobalStrings.Tab_And_Panel_Background_Color_Css_Var}:{TabsAndPanelBgColour};").Replace(";;", ";"),

            StyleInfo.TabClasses => (active) ? $"{GlobalStrings.Tab_Class} {GlobalStrings.Tab_Active_Modifier_Class}" : GlobalStrings.Tab_Class,
            StyleInfo.TabsClasses => WrapTabs ? $"{GlobalStrings.Tabs_Class} {GlobalStrings.Tabs_Wrap_Modifier_Class}" : GlobalStrings.Tabs_Class,

            StyleInfo.TabTitleClasses => GlobalStrings.Tab_Title_Class,
            StyleInfo.TabControlClasses => GlobalStrings.Tab_Control_Class,
            _ => String.Empty,
        };


    /*
         * Solution to the end of video challenge - Set tab focus programmatically and have screen readers announce it correctly.
         * ========================================
         * Make use you add the SetActiveTab method below.
         * Add a property on the Home page of type TabControl: private TabControl TabControlRef {get;set;}
         * Add a @ref="TabControlRef" directive to tab control on the home page.
         * Now just call await TabControlRef.SetActiveTab 
    */
    public async Task SetActiveTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex > _tabPages.Count -1 || _activeTabIndex == tabIndex) return;

        await RaiseTabChanged(tabIndex);                // notify parties of index change
        await _tabButtonRefs[tabIndex].FocusAsync();    // set focus on the element.

    }
}
