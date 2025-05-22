# NewsAggregatorLambda

A serverless C# AWS Lambda application that fetches daily top news from CurrentsAPI and logs a digest. Built for easy CI/CD, local testing, and deployment with AWS SAM.

---

## Features

- Fetches top news headlines from CurrentsAPI
- Runs daily on a schedule (via AWS EventBridge/SAM)
- Built with C# (.NET 6+) for AWS Lambda
- CI/CD setup using GitHub Actions
- Easily extendable for emailing, archiving, or more

---

## Prerequisites

- **AWS Account:** [Sign up](https://aws.amazon.com/free/)
- **AWS CLI:** [Install](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
  - Configure with: `aws configure`
- **AWS SAM CLI:** [Install](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html)
- **.NET 6+ SDK:** [Download](https://dotnet.microsoft.com/en-us/download)
- **Docker Desktop:** [Install](https://www.docker.com/products/docker-desktop) (for local Lambda testing)
- **CurrentsAPI Key:** [Get one free](https://currentsapi.services/en/docs/)

---

## Setup Instructions

### 1. Clone the repository

```sh
git clone https://github.com/yourusername/NewsAggregatorLambda.git
cd NewsAggregatorLambda
```

### 2. Add your CurrentsAPI Key

Set your key as an environment variable locally, or replace the value in `template.yaml` (see the Lambda environment variables section).

### 3. Build the Application

```sh
sam build
```

### 4. Test Locally (Optional)

- **Invoke Lambda locally:**
  ```sh
  sam local invoke
  ```
- **(If extending with API Gateway) Start local API:**
  ```sh
  sam local start-api
  ```

### 5. Deploy to AWS

```sh
sam deploy --guided
```
Follow the prompts to create a new stack, set the region, etc.

### 6. (Optional) Update the Schedule

Edit the `Schedule` property in `template.yaml` for when the Lambda should run.

---

## CI/CD

- GitHub Actions workflow at `.github/workflows/ci-cd.yaml` builds, tests, and deploys on push.
- Set AWS credentials as GitHub secrets:  
  `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`.

---

## Project Structure

- `src/` - Lambda code and logic
- `test/` - Unit tests
- `.github/workflows/` - CI/CD pipeline
- `template.yaml` - AWS SAM infrastructure as code

---

## Extending

- Integrate AWS SES to email the news digest.
- Store digests in DynamoDB for archiving.
- Add more endpoints or data sources.

---

## Support

Open an issue or PR for questions and improvements!