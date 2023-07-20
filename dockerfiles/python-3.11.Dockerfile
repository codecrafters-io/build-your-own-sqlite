FROM python:3.11-alpine

RUN pip install pipenv

COPY Pipfile /app/Pipfile
COPY Pipfile.lock /app/Pipfile.lock

WORKDIR /app

ENV LANG="en_US.UTF-8"
ENV PIPENV_VENV_IN_PROJECT=1

RUN pipenv install

# Force environment creation
RUN pipenv run python3 -c "import sqlparse" # Ensure the deps are available
RUN pipenv --venv

RUN mkdir -p /app-cached
RUN mv /app/.venv /app-cached/.venv

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="Pipfile,Pipfile.lock"