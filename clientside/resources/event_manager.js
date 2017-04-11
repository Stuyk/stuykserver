API.onServerEventTrigger.connect(function (event, args) {
    switch (event) {
        // Clothing Shop
        case "setupClothingMode":
            resource.clothing_mode.setupClothingMode(args[0]);
            return;
    }
});
