var x, i, j, l, ll, selElmnt, a, b, c,language;

x = document.getElementsByClassName("language-select");
l = x.length;
var select = document.querySelector('#select-lang')
language = select.value
for (i = 0; i < l; i++) {
    selElmnt = x[i].getElementsByTagName("select")[0]
    ll = selElmnt.length;

    a = document.createElement("DIV");
    a.setAttribute("class", "select-selected");
    a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
    x[i].appendChild(a);

    b = document.createElement("DIV");
    b.setAttribute("class", "select-items select-hide");
    for (j = 0; j < ll; j++) {

        c = document.createElement("DIV");
        c.innerHTML = selElmnt.options[j].innerHTML;
        c.addEventListener("click", function (e) {

            var y, i, k, s, h, sl, yl;
            s = this.parentNode.parentNode.getElementsByTagName("select")[0];
            sl = s.length;
            h = this.parentNode.previousSibling;
            for (i = 0; i < sl; i++) {
                if (s.options[i].innerHTML == this.innerHTML) {
                    s.selectedIndex = i;
                    h.innerHTML = this.innerHTML;
                    y = this.parentNode.getElementsByClassName("same-as-selected");
                    yl = y.length;
                    for (k = 0; k < yl; k++) {
                        y[k].removeAttribute("class");
                    }
                    this.setAttribute("class", "same-as-selected");
                    break;
                }
            }
            h.click();
        });
        b.appendChild(c);
    }
    x[i].appendChild(b);
    a.addEventListener("click", function (e) {

        e.stopPropagation();
        closeAllSelect(this);
        this.nextSibling.classList.toggle("select-hide");
        this.classList.toggle("select-arrow-active");
    });
}

function closeAllSelect(elmnt) {
    var select = document.querySelector('#select-lang')
    if (language != select.value)
    {
        select.dispatchEvent(new Event('change'));
        language = select.value
    }

    var x, y, i, xl, yl, arrNo = [];
    x = document.getElementsByClassName("select-items");
    y = document.getElementsByClassName("select-selected");
    xl = x.length;
    yl = y.length;
    for (i = 0; i < yl; i++) {
        if (elmnt == y[i]) {
            arrNo.push(i)
        } else {
            y[i].classList.remove("select-arrow-active");
        }
    }
    for (i = 0; i < xl; i++) {
        if (arrNo.indexOf(i)) {
            x[i].classList.add("select-hide");
        }
    }
}

document.addEventListener("click", closeAllSelect);



$('.overlay, .modal-close').click(function () {
    $('.modal, .overlay').removeClass('shown');
    location.reload();
});

$('.overlay, .modal-close').click(function () {
    $('.modal, .overlay').removeClass('shown');
});

$('.load-modal-frame').click(function () {
    let url = $(this).attr('href');
    $('.modal iframe').attr('src', url);
    $('.overlay, .modal').addClass('shown');
    return false;
});

$('.edit-iframe-form').submit(function () {
    $('.overlay, .modal').addClass('shown');
    return true;
});

$('#file').change(function () {
    $('#file-form').submit();
});

$('textarea').on('keydown', function (event) {
    var MAXLEN = $(this).attr('maxlength');
    var str = $(this).val();
    var newLines = str.split('\n').length - 1;
    var len = str.length + newLines;
    if (event.which == 13) {
        if (len > MAXLEN - 2)
            return false;
        else
            newLines++;
    }
    if (str.length >= MAXLEN - newLines) {
        return false;
    }
    else {
        return true
    }
});



