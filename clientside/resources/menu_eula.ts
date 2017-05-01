function menuEULA() {
    // Setup our menu.
    resource.menu_builder.setupMenu(1); // Setup at Index 0;
    let currentPage = 0;                // Set a local variable for the page we're working on.

    // Create a Heading Panel
    let panel = resource.menu_builder.createPanel(currentPage, 12, 4, 8, 2, true, "EULA"); // Page Number, Start X, Start Y, Width, Height, isHeading?, Text/String
    panel.setCentered(); // Center our text.

    // Create a Normal Panel
    let string_EULA = "By clicking accept you agree not to manipulate any files recieved from this server. Violation of this contract will result in immediate termination from the server. As well as a permanent restriction of access to the server. These are regulations you must abide by in order to join and play on this server. Any files recieved from this server are prohibited usage elsewhere.";
    panel = resource.menu_builder.createPanel(currentPage, 12, 6, 8, 2, false, string_EULA);
    panel.setCentered();
    // Create a Filler Panel
    panel = resource.menu_builder.createPanel(currentPage, 12, 8, 8, 2, false, "http://www.stuyk.com/ ~n~ Stuyk - 2017 - Trevor W.");
    panel.setCentered();

    // Create an Agreement Button.
    let agree = resource.menu_builder.createButton(currentPage, 12, 10, 4, 1, 1, "Agree");
    agree.function(navigateShowLogin);
    // Create a Disagree Button.
    let disagree = resource.menu_builder.createButton(currentPage, 16, 10, 4, 1, 3, "Disagree");
    disagree.function(resource.menu_eula.menu_Eula_Disconnect);

    // Open our menu.
    resource.menu_builder.openMenu(true, false, false, true, false);
}

function navigateShowLogin() {
    resource.menu_builder.exitMenu(true, false, false, false, false);
    resource.menu_login.menuLoginPanel();
}

function menu_Eula_Disconnect() {
    resource.menu_builder.exitMenu(false, true, true, true, true);
    API.disconnect("~r~Declined Agreement");
}