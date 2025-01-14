commitId=v2
docker login -u "hoangvdang" -p "Noragamiyatou@2024" docker.io

docker build -f Auth.Dockerfile -t hoangvdang/simple_microservice:auth-$commitId --label "commit.id=$commitId" --label "built.at=$(date -u)" ../../.
docker push hoangvdang/simple_microservice:auth-$commitId