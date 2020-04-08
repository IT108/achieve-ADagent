
param ($TAG, $dockerfileName)
docker build -f $dockerfileName -t iorp/achieve-adagent:$TAG .
docker push iorp/achieve-adagent:$TAG