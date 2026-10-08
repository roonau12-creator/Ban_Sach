pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
    }

    stages {

        stage('Checkout') {
            steps {
                echo '=== Checkout source code ==='
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                echo '=== Restore NuGet packages ==='
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo '=== Build project ==='
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                echo '=== Run tests ==='
                sh 'dotnet test --configuration Release --no-build'
            }
        }

        stage('Publish') {
            steps {
                echo '=== Publish application ==='
                sh 'dotnet publish --configuration Release --no-build --output ./publish'
            }
        }
    }

    post {

        success {
            echo '================================'
            echo ' BUILD SUCCESSFUL'
            echo '================================'
        }

        failure {
            echo '================================'
            echo ' BUILD FAILED'
            echo '================================'
        }

        always {
            echo '=== Pipeline finished ==='
        }
    }
}

