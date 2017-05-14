var p_Bank = null;
var p_Money = null;
var p_deposit_money: TextElement = null;
var p_deposit_bank: TextElement = null;
var p_withdraw_money: TextElement = null;
var p_withdraw_bank: TextElement = null;
var atmInputDeposit: InputPanel = null;
var atmInputWithdraw: InputPanel = null;
var isAtmOpen = false;

// Menu Variables
var menu: Menu;

API.onEntityDataChange.connect(function (entity, key, oldvalue) {
    if (!isAtmOpen) {
        return;
    }

    if (key === "P_Money") {
        if (entity.Value !== API.getLocalPlayer().Value) {
            return;
        }
        p_Money = API.getEntitySyncedData(API.getLocalPlayer(), "P_Money");
        p_deposit_money.Text = `Cash on Hand: ~g~$${p_Money}`;
        p_withdraw_money.Text = `Cash on Hand: ~g~$${p_Money}`;
        return;
    }

    if (key === "P_Bank") {
        if (entity.Value !== API.getLocalPlayer().Value) {
            return;
        }
        p_Bank = API.getEntitySyncedData(API.getLocalPlayer(), "P_Bank");
        p_deposit_bank.Text = `Balance: ~b~$${p_Bank}`;
        p_withdraw_bank.Text = `Balance: ~b~$${p_Bank}`;
    }
});

function menuATMPanel() {
    let player = API.getPlayerName(API.getLocalPlayer());
    p_Money = API.getEntitySyncedData(API.getLocalPlayer(), "P_Money");
    p_Bank = API.getEntitySyncedData(API.getLocalPlayer(), "P_Bank");
    menu = resource.menu_builder.createMenu(3);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;
    // Page 0 - Landing ========================
    panel = menu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("ATM");
    textElement.Color(255, 255, 255, 255);
    textElement.VerticalCentered = true;
    textElement.Offset = 18;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    // Exit
    for (var i = 0; i < 3; i++) {
        panel = menu.createPanel(i, 19, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Tooltip = "Exit";
        panel.Function = exitAtm;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Header = true;
        textElement = panel.addText("X");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;
        textElement.HoverColor(0, 180, 255, 255);

        if (i > 0) {
            panel = menu.createPanel(i, 12, 4, 1, 1);
            panel.MainBackgroundColor(0, 0, 0, 175);
            panel.Tooltip = "Back";
            panel.Function = goToLanding;
            panel.HoverBackgroundColor(25, 25, 25, 160);
            panel.Hoverable = true;
            panel.Header = true;
            textElement = panel.addText("<");
            textElement.Color(255, 255, 255, 255);
            textElement.Centered = true;
            textElement.VerticalCentered = true;
            textElement.FontScale = 0.6;
            textElement.HoverColor(0, 180, 255, 255);
        }

    }
    // Main Body
    panel = menu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(`Hello ${player}, what type of transaction will you be making today?`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.4;
    textElement.Font = 4;
    textElement.Centered = true;
    // Withdraw Button
    panel = menu.createPanel(0, 12, 12, 4, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = goToWithdraw;
    panel.Tooltip = "Withdraw money from your account.";
    textElement = panel.addText("Withdraw");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Deposit Button
    panel = menu.createPanel(0, 16, 12, 4, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = goToDeposit;
    panel.Tooltip = "Deposit money into your account.";
    textElement = panel.addText("Deposit");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Page 1 ================================
    // Withdraw
    panel = menu.createPanel(1, 13, 4, 6, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Withdraw");
    textElement.Color(255, 255, 255, 255);
    textElement.VerticalCentered = true;
    textElement.Offset = 18;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    // Main Body
    panel = menu.createPanel(1, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.addText("");
    p_withdraw_money = panel.addText(`Cash on Hand: ~g~$${p_Money}`);
    p_withdraw_money.Color(255, 255, 255, 255);
    p_withdraw_money.FontScale = 0.4;
    p_withdraw_money.Centered = true;
    p_withdraw_money.Font = 4;
    p_withdraw_bank = panel.addText(`Balance: ~b~$${p_Bank}`);
    p_withdraw_bank.Color(255, 255, 255, 255);
    p_withdraw_bank.FontScale = 0.4;
    p_withdraw_bank.Centered = true;
    p_withdraw_bank.Font = 4;
    atmInputWithdraw = panel.addInput(0, 3, 8, 1);
    atmInputWithdraw.NumericOnly = true;
    // The withdraw button
    panel = menu.createPanel(1, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = resource.menu_atm.withdrawCash;
    panel.Tooltip = "Withdraw input amount.";
    textElement = panel.addText("Withdraw");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Page 2 ================================
    // Deposit
    panel = menu.createPanel(2, 13, 4, 6, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Deposit");
    textElement.Color(255, 255, 255, 255);
    textElement.VerticalCentered = true;
    textElement.Offset = 18;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    // Main Body
    panel = menu.createPanel(2, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.addText("");
    p_deposit_money = panel.addText(`Cash on Hand: ~g~$${p_Money}`);
    p_deposit_money.Color(255, 255, 255, 255);
    p_deposit_money.FontScale = 0.4;
    p_deposit_money.Centered = true;
    p_deposit_money.Font = 4;
    p_deposit_bank = panel.addText(`Balance: ~b~$${p_Bank}`);
    p_deposit_bank.Color(255, 255, 255, 255);
    p_deposit_bank.FontScale = 0.4;
    p_deposit_bank.Centered = true;
    p_deposit_bank.Font = 4;
    atmInputDeposit = panel.addInput(0, 3, 8, 1);
    atmInputDeposit.NumericOnly = true;
    // The Deposit button
    panel = menu.createPanel(2, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = resource.menu_atm.depositCash;
    panel.Tooltip = "Deposit input amount.";
    textElement = panel.addText("Deposit");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    menu.Blur = true;
    menu.DisableOverlays(true);
    menu.Ready = true;
    isAtmOpen = true;
}

function exitAtm() {
    isAtmOpen = false;
    menu.killMenu();
}

function goToLanding() {
    menu.Page = 0;
}

function goToWithdraw() {
    menu.Page = 1;
}

function goToDeposit() {
    menu.Page = 2;
}

function atmSuccess() {
    panel = resource.menu_builder.createNotification(0, "~g~You have made a successful transaction.", 3000);
    panel.setColor(0, 255, 0);
}

function depositCash() {
    let currentInput = atmInputDeposit.Input;
    if (p_Money < 1) {
        atmInputDeposit.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You don't have that much money to deposit.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }

    // Check if anything is input at all.
    if (currentInput.length < 1) {
        atmInputDeposit.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You must insert a value to deposit.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }
    // Less than Zero
    if (Number(currentInput) <= 0) {
        atmInputDeposit.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You must insert a value greater than 0.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }
    // Greater than Max 32 Bit
    if (Number(currentInput) > 2147483647) {
        atmInputDeposit.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Input exceeds max limit.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }

    atmInputDeposit.isError = false;
    API.triggerServerEvent("depositATM_Server", currentInput);
}

function withdrawCash() {
    let currentInput = atmInputWithdraw.Input;
    if (p_Bank < 1) {
        atmInputWithdraw.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You don't have any money in the bank.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }
    // Check if anything is input at all.
    if (currentInput.length < 1) {
        atmInputWithdraw.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You must insert a value to withdraw.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }
    // Less than Zero
    if (Number(currentInput) <= 0) {
        atmInputWithdraw.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~You must insert a positive value.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }
    // Greater than Max 32 Bit
    if (Number(currentInput) > 2147483647) {
        atmInputWithdraw.isError = true;
        panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Input exceeds max limit.", 3000);
        panel.setColor(255, 0, 0);
        return;
    }

    atmInputWithdraw.isError = false;
    API.triggerServerEvent("withdrawATM_Server", currentInput);
}