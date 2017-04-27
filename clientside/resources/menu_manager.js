function menuEULA() {
    let pane = resource.menu_builder.addPane(12, 1, 8, 2, true, "EULA");
    pane.setCentered();
    var textEula = "By clicking accept you agree not to manipulate any files recieved from this server. Violation of this contract will result in immediate termination from the server. As well as a permanent restriction of access to the server. These are regulations you must abide by in order to join and play on this server. Any files recieved from this server are prohibited usage elsewhere.";
    pane = resource.menu_builder.addPane(12, 3, 8, 2, false, textEula);
    pane.setCentered();
    pane = resource.menu_builder.addPane(12, 5, 8, 2, false, "http://www.stuyk.com/ ~n~ Stuyk - 2017 - Trevor W.");
    pane.setCentered();
    let agree = resource.menu_builder.createButton(12, 7, 4, 1, 1, "Agree");
    agree.function(resource.menu_manager.menu_Eula_ShowLogin);
    let disagree = resource.menu_builder.createButton(16, 7, 4, 1, 3, "Disagree");
    disagree.function(resource.menu_manager.menu_Eula_Disconnect);
    resource.menu_builder.openMenu(true, false, false, true);
}
function menu_Eula_ShowLogin() {
    resource.menu_builder.exitMenu(true, false, false, false);
    resource.browser_manager.showCEF("clientside/resources/index.html");
}
function menu_Eula_Disconnect() {
    resource.menu_builder.exitMenu(true, false, false, false);
    API.disconnect("~r~Declined Agreement");
}
