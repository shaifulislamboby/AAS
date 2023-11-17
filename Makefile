build:
	docker build --no-cache -t mes_aas:v1 .

start:
	docker run -it -p 6999:6999 -p 6499:6499 -p 443:443 -p 5499:5499  mes_aas:v1
install:
	docker run -it -v ${PWD}:/app -w /app -u node node npm ci

format:
	docker run -it -v ${PWD}:/app -w /app -u node node npx prettier --write .

db-shell:
	docker compose exec db psql postgres postgres
