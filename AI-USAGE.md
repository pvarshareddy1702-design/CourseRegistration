# AI-Assisted Engineering Record

## 1. AI tool used

**Tool:** ChatGPT

ChatGPT was used as an AI engineering assistant throughout the assessment workflow, beginning with project setup and environment preparation and continuing through implementation, testing, and documentation.

## 2. Activities supported by AI

### Project setup and environment preparation
- Received step-by-step guidance to prepare the development environment in Visual Studio Code.
- Diagnosed the missing .NET SDK and installed or configured the SDK needed for development.
- Received guidance on GitHub authentication and preparing the project for repository-based submission.

### Design and implementation
- Discussed the assessment requirements and translated them into enrollment, prerequisite, capacity, and waitlist business rules.
- Used AI-generated implementation guidance for the `EnrollmentService`, including duplicate prevention, enrollment and drop operations, FIFO promotion, and synchronization.
- Adopted the policy of skipping ineligible waitlisted learners during promotion while retaining their queue entries.

### Testing
- Used AI assistance to develop unit tests for available capacity, full capacity, duplicate registration, FIFO promotion, prerequisite failures, invalid requests, repeated drops, ineligible waitlist entries, and concurrent enrollment.
- Ran the test suite locally to check the implementation.

### Documentation
- Used AI assistance to draft the README, design specification, and this AI-use record.

## 3. Human review and decisions

The developer reviewed the proposed requirements and implementation steps, entered the code into the local project, and ran the build and test commands.

The implementation and documentation were developed iteratively. Suggestions were used as a starting point and should be reviewed against the final code and actual test results before submission.

## 4. Verification evidence

Record the final results from the local environment before submitting:

- Build command: `dotnet build`
- Test command: `dotnet test`
- Test result observed during development: 13 tests passed.
- Final build and test results: [Update after the final verification run.]

## 5. Limitations

The prototype uses in-memory state and supports one course offering per service instance. Data is not persisted across restarts. The implementation has not been represented as production-deployed or integrated with a real course-management system.

## 6. Transparency

This record describes the AI assistance used during development. The final repository should reflect the actual implementation, decisions, and verification performed by the developer. Any additional AI tools used should be added to this record.
