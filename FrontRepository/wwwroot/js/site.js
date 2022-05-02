// кнопка "все песни"
function moreDetailed() {
    var p = document.getElementById("flipflop");
    var p2 = document.getElementsByName("AreChecked");

    if (p.checked === true) p2.forEach(function (entry) { entry.checked = true; });
    if (p.checked === false) p2.forEach(function (entry) { entry.checked = false; });
    
}

// прячет-и возвращает меню
function hideMenu() {
    var product = document.getElementById("btns");
    var hider = document.getElementById("hider");

    if (hider.checked === false) {
        product.style.display = "none";
        hider.checked = true;
        return;
    }
    if (hider.checked === true) {
        product.style.display = "block";
        hider.checked = false;
    }
}

export default moreDetailed;