document.addEventListener('click', function (e) {
    if (e.target.classList.contains("add-menu")) {
        ++e.target.parentElement.querySelector("input").value;
    } else if (e.target.classList.contains("remove-menu") && e.target.parentElement.querySelector("input").value > 0) {
        --e.target.parentElement.querySelector("input").value;
    }
})