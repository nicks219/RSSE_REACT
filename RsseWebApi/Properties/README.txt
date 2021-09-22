вручную yarn run build
после этого запустится Production
перенеси css в src:
import './bootstrap.css';
import './react.css';

spa.UseReactDevelopmentServer(npmScript: "build");
ccылка на скрипты в package.json
скрипт "react-scripts build" (npm run build)
построил билд, но не запустился kestrel
- возможно скрипт не awaitable

убрал  "proxy": "http://localhost:5000" из package.json

добавил скрипт cd ./ClientApp/build || cd ./ClientApp && yarn run build в BuildEvent (свойства проекта)
сделал работающий метод YarnRunBuild()