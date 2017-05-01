// Login Inputs
var login_user_input = null;
var login_pass_input = null;
// Registration Inputs
var reg_user_input = null;
var reg_pass_input = null;
var reg_pass_verify_input = null;
//
var isActive = false;
var tabIndex = 0;
// Main Page Login
API.onUpdate.connect(function () {
    if (!isActive) {
        return;
    }
    // Tab
    if (API.isControlJustPressed(37 /* SelectWeapon */)) {
        var currentPage = resource.menu_builder.getCurrentPage();
        if (Number.parseInt(currentPage) === 1) {
            if (tabIndex === 0) {
                tabIndex = 1;
                login_user_input.setSelected();
                login_pass_input.setUnselected();
            }
            else {
                tabIndex = 0;
                login_user_input.setUnselected();
                login_pass_input.setSelected();
            }
        }
        if (Number.parseInt(currentPage) === 2) {
            if (tabIndex === 0) {
                tabIndex = 1;
                reg_user_input.setSelected();
                reg_pass_input.setUnselected();
                reg_pass_verify_input.setUnselected();
            }
            else if (tabIndex === 1) {
                tabIndex = 2;
                reg_user_input.setUnselected();
                reg_pass_input.setSelected();
                reg_pass_verify_input.setUnselected();
            }
            else {
                tabIndex = 0;
                reg_user_input.setUnselected();
                reg_pass_input.setUnselected();
                reg_pass_verify_input.setSelected();
            }
        }
    }
    if (API.isControlJustPressed(176 /* PhoneSelect */)) {
        let currentPage = resource.menu_builder.getCurrentPage();
        if (Number.parseInt(currentPage) === 1) {
            attemptLogin();
        }
        else {
            attemptRegister();
        }
    }
});
function menuLoginPanel() {
    let panel;
    let login;
    let register;
    let args;
    let button;
    // Setup our menu with 3 pages. 0 - 2
    resource.menu_builder.setupMenu(4);
    panel = resource.menu_builder.createPanel(0, 12, 4, 8, 2, true, "Stuyk RP"); // Page Number, Start X, Start Y, Width, Height, isHeading?, Text/String
    panel.setCentered(); // Center our text.
    panel = resource.menu_builder.createPanel(1, 13, 4, 7, 2, true, "Login"); // Page Number, Start X, Start Y, Width, Height, isHeading?, Text/String
    panel = resource.menu_builder.createPanel(2, 13, 4, 7, 2, true, "Register"); // Page Number, Start X, Start Y, Width, Height, isHeading?, Text/String
    // Generate static elements for 3 pages.
    for (var i = 0; i < 3; i++) {
        if (i > 0 && i < 3) {
            button = resource.menu_builder.createButton(i, 12, 4, 1, 2, 0, "<");
            args = [0];
            button.addArgs(args);
            button.function(resource.menu_builder.setPage);
        }
    }
    // For Page 0
    panel = resource.menu_builder.createPanel(0, 12, 6, 8, 6, false, "Are you a new or existing user?");
    panel.setCentered();
    panel.setVerticalCentered();
    panel.setFontScale(0.6);
    login = resource.menu_builder.createButton(0, 12, 12, 4, 1, 0, "Existing User");
    args = [1];
    login.addArgs(args);
    login.function(resource.menu_builder.setPage);
    register = resource.menu_builder.createButton(0, 16, 12, 4, 1, 0, "New User");
    args = [2];
    register.addArgs(args);
    register.function(resource.menu_builder.setPage);
    // For Page 1 - Login
    // Username Field
    panel = resource.menu_builder.createPanel(1, 12, 6, 8, 1, false, "Username");
    panel.setFontScale(0.6);
    login_user_input = resource.menu_builder.createInput(1, 12, 7, 8, 1, false, false);
    // Password Field
    panel = resource.menu_builder.createPanel(1, 12, 8, 8, 1, false, "Password");
    panel.setFontScale(0.6);
    login_pass_input = resource.menu_builder.createInput(1, 12, 9, 8, 1, true, false);
    // Spacer
    panel = resource.menu_builder.createPanel(1, 12, 10, 8, 3, false, "");
    button = resource.menu_builder.createButton(1, 12, 13, 8, 1, 0, "Login");
    button.function(resource.menu_login.attemptLogin);
    // For Page 2 - Register
    // Username Field
    panel = resource.menu_builder.createPanel(2, 12, 6, 8, 1, false, "Username");
    panel.setFontScale(0.6);
    reg_user_input = resource.menu_builder.createInput(2, 12, 7, 8, 1, false, false);
    // Password Field
    panel = resource.menu_builder.createPanel(2, 12, 8, 8, 1, false, "Password");
    panel.setFontScale(0.6);
    reg_pass_input = resource.menu_builder.createInput(2, 12, 9, 8, 1, true, false);
    panel = resource.menu_builder.createPanel(2, 12, 10, 8, 1, false, "Password Again");
    panel.setFontScale(0.6);
    reg_pass_verify_input = resource.menu_builder.createInput(2, 12, 11, 8, 1, true, false);
    // Spacer
    panel = resource.menu_builder.createPanel(2, 12, 12, 8, 1, false, "");
    button = resource.menu_builder.createButton(2, 12, 13, 8, 1, 0, "Register");
    button.function(resource.menu_login.attemptRegister);
    // For Page 3
    panel = resource.menu_builder.createPanel(3, 12, 8, 8, 2, false, "Please Wait...");
    panel.setFontScale(0.6);
    panel.setCentered();
    panel.setVerticalCentered();
    resource.menu_builder.openMenu(true, true, true, true, false);
    isActive = true;
}
function attemptLogin() {
    let user = login_user_input.returnInput();
    let pass = login_pass_input.returnInput();
    if (user.length < 5) {
        login_user_input.isError(true);
        return;
    }
    login_user_input.isError(false);
    if (pass.length < 5) {
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        login_pass_input.isError(true);
        return;
    }
    else {
        login_pass_input.isError(false);
        API.playSoundFrontEnd("CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET");
        API.triggerServerEvent("clientLogin", user, pass);
        resource.menu_builder.setPage(3);
        return;
    }
}
function attemptRegister() {
    let user = reg_user_input.returnInput();
    let pass = reg_pass_input.returnInput();
    let pass_verify = reg_pass_verify_input.returnInput();
    if (user.length < 5) {
        reg_user_input.isError(true);
        return;
    }
    reg_user_input.isError(false);
    if (pass.length < 5 && pass_verify.length < 5) {
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        reg_pass_input.isError(true);
        reg_pass_verify_input.isError(true);
        return;
    }
    reg_pass_input.isError(false);
    reg_pass_verify_input.isError(false);
    if (pass === pass_verify) {
        API.playSoundFrontEnd("CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET");
        API.triggerServerEvent("clientRegistration", user, pass);
        resource.menu_builder.setPage(3);
        return;
    }
    else {
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        reg_pass_input.isError(true);
        reg_pass_verify_input.isError(true);
        return;
    }
}
