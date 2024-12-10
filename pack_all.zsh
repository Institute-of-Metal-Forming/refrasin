#!/usr/bin/env zsh

export RE_MATCH_PCRE=1
output_path=./publish

for package in RefraSin.*
do
    if [[ ! ( $package =~ .*\.Tests?) ]]
    then
        echo
        echo '---' $package '---'
        dotnet pack "$package" -o $output_path
    fi
done
