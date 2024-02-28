docker build --tag broker .
docker run --rm -it -p 8000:8080 --name broker broker