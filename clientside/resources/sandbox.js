API.onResourceStart.connect(function () {
    let menu = resource.menu_builder.createMenu(2);
    let panel;
    let inputPanel;
    let textElement;
    // Create buttons / headers for all pages.
    for (var i = 0; i < 2; i++) {
        // Header Panels
        panel = menu.createPanel(i, 12, 1, 8, 2);
        panel.MainBackgroundColor(92, 49, 4, 255);
        panel.Header = true;
        textElement = panel.addText(`Menu Builder`);
        textElement.Centered = true;
        textElement.Font = 1;
        textElement.Color(206, 213, 206, 255);
        textElement = panel.addText(`Page ( ${i} / 1 )`);
        textElement.Font = 4;
        textElement.Color(206, 213, 206, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.45;
        // Previous Page
        panel = menu.createPanel(i, 11, 1, 1, 2);
        panel.MainBackgroundColor(92, 49, 4, 255);
        panel.Function = menu.prevPage;
        panel.Header = true;
        panel.Hoverable = true;
        panel.HoverBackgroundColor(154, 110, 64, 255);
        panel.Tooltip = "Previous Page";
        textElement = panel.addText("<");
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.Color(206, 213, 206, 255);
        // Next Page
        panel = menu.createPanel(i, 20, 1, 1, 2);
        panel.MainBackgroundColor(92, 49, 4, 255);
        panel.Function = menu.nextPage;
        panel.Header = true;
        panel.Hoverable = true;
        panel.HoverBackgroundColor(154, 110, 64, 255);
        panel.Tooltip = "Next Page";
        textElement = panel.addText(">");
        textElement.VerticalCentered = true;
        textElement.Centered = true;
        textElement.Color(206, 213, 206, 255);
    }
    panel = menu.createPanel(0, 11, 3, 10, 10);
    panel.MainBackgroundColor(206, 213, 206, 255);
    // Line Element 0
    textElement = panel.addText(`Line 0`);
    textElement.Centered = true;
    textElement.Font = 4;
    textElement.FontScale = 0.4;
    textElement.Color(83, 117, 40, 255);
    // Line Element 1
    textElement = panel.addText(`Line 1`);
    textElement.Centered = true;
    textElement.Font = 4;
    textElement.FontScale = 0.4;
    textElement.Color(83, 117, 40, 255);
    // Line Element 2
    textElement = panel.addText(`Line 2`);
    textElement.Centered = true;
    textElement.Font = 4;
    textElement.FontScale = 0.4;
    textElement.Color(83, 117, 40, 255);
    // Example Function Button
    panel = menu.createPanel(0, 11, 13, 10, 1);
    panel.MainBackgroundColor(47, 76, 22, 255);
    panel.Hoverable = true;
    panel.Header = true;
    panel.Function = testMessage;
    panel.HoverBackgroundColor(83, 117, 40, 255);
    textElement = panel.addText(`Fire Event`);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.Font = 4;
    textElement.FontScale = 0.5;
    textElement.Color(206, 213, 206, 255);
    API.showCursor(true);
    // Second Page Input Panel Example / Image
    panel = menu.createPanel(1, 11, 3, 10, 5);
    panel.MainBackgroundColor(206, 213, 206, 255);
    panel.MainBackgroundImagePadding = 10;
    panel.MainBackgroundImage = "clientside/resources/images/backgrounds/background_2.jpg";
    panel = menu.createPanel(1, 11, 8, 10, 5);
    panel.MainBackgroundColor(206, 213, 206, 255);
    textElement = panel.addText(`Regular Input`);
    textElement.Color(83, 117, 40, 255);
    textElement = panel.addText(` `);
    inputPanel = panel.addInput(0, 1, 10, 1);
    inputPanel.MainBackgroundColor(0, 0, 0, 100);
    inputPanel.HoverBackgroundColor(0, 0, 0, 75);
    inputPanel.SelectBackgroundColor(0, 0, 0, 125);
    textElement = panel.addText(`Numeric Input`);
    textElement.Color(83, 117, 40, 255);
    inputPanel = panel.addInput(0, 3, 10, 1);
    inputPanel.NumericOnly = true;
    inputPanel.MainBackgroundColor(0, 0, 0, 100);
    inputPanel.HoverBackgroundColor(0, 0, 0, 75);
    inputPanel.SelectBackgroundColor(0, 0, 0, 125);
    menu.Ready = true;
});
/**
 *  This is a test function, to help understand how the menu builder handles functions.
 */
function testMessage() {
    API.sendChatMessage("This is a test function.");
}
