# Copilot Instructions

## Project Guidelines
- Użytkownik oczekuje ścisłego CQRS: kontrolery mają delegować przez MediatR i nie powinny bezpośrednio korzystać z warstwy infrastruktury (DB/Redis).
- Użytkownik preferuje, aby budowy Angular/frontend były uruchamiane całkowicie w Dockerze, bez instalacji npm lokalnie.