вручную yarn run build
после этого запустится Production
перенеси css в src:
import './bootstrap.css';
import './react.css';

spa.UseReactDevelopmentServer(npmScript: "build");
ccылка на скрипты в package.json
скрипт "react-scripts build" (npm run build) из реакта не справился
скрипт построил билд, но не смог запустить kestrel

см п.1 
вручную yarn run build