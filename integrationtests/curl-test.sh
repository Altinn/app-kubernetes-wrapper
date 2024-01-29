#!/bin/sh
kubectl wait deployment -n default kuberneteswrapper --for condition=Available=True --timeout=90s
kubectl port-forward svc/kuberneteswrapper 8080:8080 &
PORTFORWARD_PID=$!
sleep 5 # Wait for portforward to be ready
expectedDeployment='[{"version":"1.23.2-alpine","release":"dummy-deployment","status":{"availableReplicas":1,"collisionCount":null,"conditions":[{"message":"Deployment has minimum availability.","reason":"MinimumReplicasAvailable","status":"True","type":"Available"},{"message":"ReplicaSet \"dummy-deployment-b9b48b788\" has successfully progressed.","reason":"NewReplicaSetAvailable","status":"True","type":"Progressing"}],"observedGeneration":1,"readyReplicas":1,"replicas":1,"unavailableReplicas":null,"updatedReplicas":1},"replicas":1},{"version":"local","release":"kuberneteswrapper","status":{"availableReplicas":1,"collisionCount":null,"conditions":[{"message":"Deployment has minimum availability.","reason":"MinimumReplicasAvailable","status":"True","type":"Available"},{"message":"ReplicaSet \"kuberneteswrapper-6d4c57fc98\" has successfully progressed.","reason":"NewReplicaSetAvailable","status":"True","type":"Progressing"}],"observedGeneration":1,"readyReplicas":1,"replicas":1,"unavailableReplicas":null,"updatedReplicas":1},"replicas":1}]'
expectedDaemonsets='[{"version":"1.23.2-alpine","release":"dummy-daemonset","status":null,"replicas":null}]'
actualDeployment=$(curl http://localhost:8080/api/v1/deployments --silent | awk '{gsub(/"lastTransitionTime":"[^"]+",/,""); gsub(/"lastUpdateTime":"[^"]+",/,"")}1')

if [[ $expectedDeployment = $actualDeployment ]]
then
  echo "Deployment test failed"
  echo "Exp: $expectedDeployment"
  echo "Got: $actualDeployment"
  exit 1
else
  echo "Deployment tests ran OK"
fi

actualDaemonsets=$(curl http://localhost:8080/api/v1/daemonsets --silent)
if [[ $expectedDaemonsets = $actualDaemonsets ]]
then
  echo "DaemonsetsTest failed"
  echo "Exp: $expectedDaemonsets"
  echo "Got: $actualDaemonsets"
  exit 1
else
  echo "Daemonsets tests ran OK"
fi
kill $PORTFORWARD_PID
