var atm_input_deposit = null;
var atm_input_withdraw = null;
var p_Bank = null;
var p_Money = null;
var p_deposit_money = null;
var p_deposit_bank = null;
var p_withdraw_money = null;
var p_withdraw_bank = null;
var isAtmOpen = false;

API.onEntityDataChange.connect(function (entity, key, oldvalue) {
    if (!isAtmOpen) {
        return;
    }

    if (key === "P_Money") {
        if (entity.Value !== API.getLocalPlayer().Value) {
            return;
        }
        p_Money = API.getEntitySyncedData(API.getLocalPlayer(), "P_Money");
        p_deposit_money.setText(`Cash on Hand: ~g~$${p_Money}`);
        p_withdraw_money.setText(`Cash on Hand: ~g~$${p_Money}`);
        return;
    }

    if (key === "P_Bank") {
        if (entity.Value !== API.getLocalPlayer().Value) {
            return;
        }
        p_Bank = API.getEntitySyncedData(API.getLocalPlayer(), "P_Bank");
        p_deposit_bank.setText(`Balance: ~b~$${p_Bank}`);
        p_withdraw_bank.setText(`Balance: ~b~$${p_Bank}`);
    }
    
    
});

function menuATMPanel() {
    let panel;
    let button;
    let args;
    let player = API.getPlayerName(API.getLocalPlayer()).replace(/_/g, " ");
    // Pull some Synced Data
    p_Money = API.getEntitySyncedData(API.getLocalPlayer(), "P_Money");
    p_Bank = API.getEntitySyncedData(API.getLocalPlayer(), "P_Bank");
    // How many pages will our menu have?
    // In this case we're initiating 3 ages. Index Values: 0, 1, 2
    resource.menu_builder.setupMenu(3);
    // Let's add our panel headers. 
    // This is for Page 1. Index 0
    panel = resource.menu_builder.createPanel(0, 12, 4, 7, 2, true, "ATM"); // Page Number, Grid X, Grid Y, Width, Height, is This A Header?, "Text"
    panel.setCentered(); // Center our Text Horizontally
    panel.setOffset(30); // Add some extra offset to our ATM text.
    // This is for Page 2. Index 1
    panel = resource.menu_builder.createPanel(1, 13, 4, 6, 2, true, "Deposit");
    panel.setCentered(); // Center our Text Horizontally
    // This is for Page 3. Index 2 <-- Is this making sense yet?
    panel = resource.menu_builder.createPanel(2, 13, 4, 6, 2, true, "Withdraw");
    panel.setCentered(); // Center our Text Horizontally
    // This is for Page 1. Index 0
    panel = resource.menu_builder.createPanel(0, 12, 6, 8, 5, false, `Hello ${ player }, what type of transaction will you be making today?`); // Page Number, Grid X, Grid Y, Width, Height, is This A Header?, Your Text
    panel.setCentered(); // Center our Text Horizontally
    panel.setVerticalCentered(); // Center our Text Vertically
    // This is for Page 1. Index 0
    button = resource.menu_builder.createButton(0, 12, 11, 4, 1, 0, "Withdraw"); // Page Number, Grid X, Grid Y, Width, Height, Type, "Text"
    args = [2]; // Withdraw is on Index 2.
    button.addArgs(args); // Push our array of arguments to the function. These are optional btw.
    button.function(resource.menu_builder.setPage) // Setup a custom function. Always use resource.FILE_NAME to call your functions.
    // This is for Page 1. Index 0
    button = resource.menu_builder.createButton(0, 16, 11, 4, 1, 0, "Deposit"); // Page Number, Grid X, Grid Y, Width, Height, Type, "Text"
    args = [1]; // Deposit is on Index 1.
    button.addArgs(args); // Push our array of arguments to the function. These are optional btw.
    button.function(resource.menu_builder.setPage) // Setup a custom function. Always use resource.FILE_NAME to call your functions.'
    // This is for Page 2. Index 1 - Deposit Panel
    p_deposit_money = resource.menu_builder.createPanel(1, 12, 6, 8, 1, false, `Cash on Hand: ~g~$${p_Money}`);
    p_deposit_money.setCentered();
    p_deposit_money.setFontScale(0.6);
    p_deposit_bank = resource.menu_builder.createPanel(1, 12, 7, 8, 1, false, `Balance: ~b~$${p_Bank}`);
    p_deposit_bank.setCentered();
    p_deposit_bank.setFontScale(0.6);
    atm_input_deposit = resource.menu_builder.createInput(1, 12, 8, 7, 1, false, false);
    atm_input_deposit.setNumericOnly();
    button = resource.menu_builder.createButton(1, 19, 8, 1, 1, 1, ">"); // Page Number, Grid X, Grid Y, Width, Height, Type, "Text"
    button.function(resource.menu_atm.depositCash);


    // This is for Page 3. Index 2 - Withdraw Panel
    p_withdraw_money = resource.menu_builder.createPanel(2, 12, 6, 8, 1, false, `Cash on Hand: ~g~$${p_Money}`);
    p_withdraw_money.setCentered();
    p_withdraw_money.setFontScale(0.6);
    p_withdraw_bank = resource.menu_builder.createPanel(2, 12, 7, 8, 1, false, `Balance: ~b~$${p_Bank}`);
    p_withdraw_bank.setCentered();
    p_withdraw_bank.setFontScale(0.6);
    atm_input_withdraw = resource.menu_builder.createInput(2, 12, 8, 7, 1, false, false);
    atm_input_withdraw.setNumericOnly();
    button = resource.menu_builder.createButton(2, 19, 8, 1, 1, 1, ">"); // Page Number, Grid X, Grid Y, Width, Height, Type, "Text"
    button.function(resource.menu_atm.withdrawCash);

    // Let's add some back buttons to get back to our first page.
    for (var i = 0; i < 3; i++) {
        button = resource.menu_builder.createButton(i, 19, 4, 1, 2, 3, "X");
        button.function(closeATM)

        if (i > 0) {
            button = resource.menu_builder.createButton(i, 12, 4, 1, 2, 0, "<");
            args = [0];
            button.addArgs(args);
            button.function(resource.menu_builder.setPage)
        }
    }
    resource.menu_builder.openMenu(true, false, false, true, false);
    isAtmOpen = true;
}

function closeATM() {
    isAtmOpen = false;
    resource.menu_builder.killMenu();
}

function atmSuccess() {
    API.playSoundFrontEnd("Checkpoint_Cash_Hit", "GTAO_FM_Events_Soundset");
}

function depositCash() {
    let currentInput = atm_input_deposit.returnInput();
    if (p_Money < 1) {
        atm_input_deposit.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }

    // Check if anything is input at all.
    if (currentInput.length < 1) {
        atm_input_deposit.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }
    // Less than Zero
    if (currentInput <= 0) {
        atm_input_deposit.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }
    // Greater than Max 32 Bit
    if (currentInput > 2147483647) {
        atm_input_deposit.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }

    atm_input_deposit.isError(false);
    API.triggerServerEvent("depositATM_Server", currentInput);
}

function withdrawCash() {
    let currentInput = atm_input_withdraw.returnInput();
    if (p_Bank < 1) {
        atm_input_withdraw.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }
    // Check if anything is input at all.
    if (currentInput.length < 1) {
        atm_input_withdraw.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }
    // Less than Zero
    if (currentInput <= 0) {
        atm_input_withdraw.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }
    // Greater than Max 32 Bit
    if (currentInput > 2147483647) {
        atm_input_withdraw.isError(true);
        API.playSoundFrontEnd("CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET");
        return;
    }

    atm_input_withdraw.isError(false);
    API.triggerServerEvent("withdrawATM_Server", currentInput);
}