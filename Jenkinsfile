pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '''echo "Build phase"

UNITY_APP_PATH=/Applications/Unity/Hub/Editor/2019.1.0b4/Unity.app/Contents/MacOS/Unity
UNITY_PROJECT_PATH=${WORKSPACE}
UNITY_BATCH_EXECUTE_METHOD=BatchBuild.Build
UNITY_EDITOR_LOG_PATH=~/Library/Logs/Unity/Editor.log

exit 0'''
      }
    }
    stage('Deploy') {
      steps {
        sh 'echo "Deply"'
      }
    }
  }
}