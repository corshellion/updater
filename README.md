# Silent Print
## How to Implement Silent Print to SLS
1. Install `Silent Print.exe` to each PC/Laptops
2. File that need to be modified once:
   1. add `libPagesMenubarTransactionSP()` in `lib/object/pages.php`
   2. add `libPrintUri()` in `lib/object/pages.php`
   3. modify `libPagesMenubarPrint()` in `lib/object/js/object_lib_pages.js`
   4. add `getversion.php` in `print_layout/`
   5. add `checkLoginByLoginID()` in modul `Home/Login`
3. Files that need to be modified 
   1. add `get.php` file to every print layout folder
   2. In every module, modify `libPagesMenubarTransaction()` to `libPagesMenubarTransactionSP()`
   3. modify `file.php` every print layout

### Important File
- `view.php` each Module
- `file.php` each print layout
- `get.php` every print layout folder
- `lib/object/pages.php`, `lib/object/js/object_lib_pages.js` & `lib/object/pages_ajax.php`
- `getversion.php` in print_layout folder
- `check_print_authorization.php` in print_layout folder


## Possible Error
- **Error 404**, Invalid getversion.php file