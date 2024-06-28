# syntax=docker/dockerfile:1.7-labs
FROM jfloff/alpine-python:3.8

RUN pip install --no-cache-dir "pipenv>=2023.12.1"


WORKDIR /app
# .git & README.md are unique per-repository. We ignore them on first copy to prevent cache misses
COPY --exclude=.git --exclude=README.md . /app

ENV LANG="en_US.UTF-8"
ENV PIPENV_VENV_IN_PROJECT=1

RUN pipenv install

# Force environment creation
RUN pipenv run python3 -c "import sqlparse" # Ensure the deps are available
RUN pipenv --venv

RUN mkdir -p /app-cached
RUN mv /app/.venv /app-cached/.venv

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="Pipfile,Pipfile.lock"

# Once the heave steps are done, we can copy all files back
COPY . /app
