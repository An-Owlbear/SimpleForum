function openDropdown(object) {
    object.classList.add("dropdown-clicked");
    object.removeEventListener('click', object.openDropdown);
    object.addEventListener('click', object.closeDropdown);

    var dropdownItems = object.getElementsByClassName("dropdown-items");
    Array.from(dropdownItems).forEach(function (element) {
        element.classList.add("dropdown-items-visible");
    })
}

function closeDropdown(object) {
    object.classList.remove("dropdown-clicked");
    object.removeEventListener('click', object.closeDropdown);
    object.addEventListener('click', object.openDropdown);

    var dropdownItems = object.getElementsByClassName("dropdown-items");
    Array.from(dropdownItems).forEach(function (element) {
        element.classList.remove("dropdown-items-visible");
    })
}

var dropdowns = document.getElementsByClassName("dropdown");

Array.from(dropdowns).forEach(function (element) {
    element.openDropdown = () => openDropdown(element);
    element.closeDropdown = () => closeDropdown(element);
    element.addEventListener("click", element.openDropdown);
})