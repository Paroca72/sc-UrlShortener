<%@ Page Title="" Language="VB" MasterPageFile="~/Masters/Private.master" AutoEventWireup="false" CodeFile="URLs.aspx.vb" Inherits="private_URLs" %>

<%@ MasterType VirtualPath="~/Masters/Private.master" %>
<%@ Register Assembly="SCFramework" Namespace="SCFramework.WebControls" TagPrefix="SCFramework" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHFilters" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPHMiddle" runat="Server">
    <!-- Telerik
    ================================================== -->
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="GridURLs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridURLs" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <!-- Row
    ================================================== -->
    <div class="row">
        <!-- Content
        ================================================== -->
        <div class="col-md-12">
            <section>
            </section>
            <telerik:RadGrid ID="GridURLs" runat="server" AutoGenerateColumns="False" CellSpacing="0"
                GridLines="None" ShowStatusBar="True">
                <ExportSettings>
                    <Pdf PageWidth="">
                    </Pdf>
                </ExportSettings>
                <MasterTableView DataKeyNames="ID_URL" EditMode="InPlace" NoMasterRecordsText="Empty grid"
                    CommandItemDisplay="Bottom" PageSize="50" InsertItemDisplay="Bottom"
                    InsertItemPageIndexAction="ShowItemOnLastPage" AllowPaging="True">
                    <CommandItemSettings AddNewRecordText=" Add new URL" ShowRefreshButton="false"></CommandItemSettings>
                    <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="ID_URL" FilterControlAltText="Filter IDColumn column"
                            HeaderText="ID" UniqueName="IDColumn" ReadOnly="true" Visible="false">
                            <ItemStyle BackColor="WhiteSmoke" VerticalAlign="Top" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="URL" FilterControlAltText="Filter URLColumn column"
                            HeaderText="Long" MaxLength="100" UniqueName="NameColumn">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter ShortColumn column" UniqueName="ShortColumn"
                            HeaderText="Short" DefaultInsertValue="True" ReadOnly="true">
                            <ItemTemplate>
                                <a href="<%# Eval("SHORT")%>" target="_blank"><%# Eval("SHORT")%></a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridEditCommandColumn FilterControlAltText="Filter EditCommandColumn column"
                            CancelText="Cancel" EditText="Edit" UpdateText="Update" InsertText="Inserisci">
                            <ItemStyle HorizontalAlign="Center" Wrap="false" VerticalAlign="Top" />
                            <HeaderStyle Width="1px" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridTemplateColumn UniqueName="DeleteColumn" ReadOnly="true">
                            <ItemTemplate>
                                <asp:LinkButton ID="LBDelete" runat="server" CommandName="Delete">Delete</asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle Width="1px" />
                            <ItemStyle HorizontalAlign="Center" Wrap="false" VerticalAlign="Top" />
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings>
                        <EditColumn FilterControlAltText="Filter EditCommandColumn1 column" UniqueName="EditCommandColumn1"
                            CancelText="Cancel" EditText="Edit" UpdateText="Update" InsertText="Insert">
                        </EditColumn>
                    </EditFormSettings>
                    <PagerStyle AlwaysVisible="True" />
                </MasterTableView>
                <PagerStyle Mode="Slider" />
                <FilterMenu EnableImageSprites="False">
                </FilterMenu>
            </telerik:RadGrid>
        </div>
    </div>
</asp:Content>
