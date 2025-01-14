commitId=`git rev-parse HEAD | cut -c 1-10`
docker login -u "hoangvdang" -p "Noragamiyatou@2024" docker.io

docker build -f Gateway.Dockerfile -t hoangvdang/simple_microservice:gateway-$commitId --label "commit.id=$commitId" --label "built.at=$(date -u)" ../../.
docker push hoangvdang/simple_microservice:gateway-$commitId