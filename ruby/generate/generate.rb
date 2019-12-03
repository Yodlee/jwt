# Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
# Licensed under the MIT License. See `LICENSE` for details.

# USAGE
#
#   generate.rb -k path/to/private.key -i issuer-id [-u username]
#

require 'jwt'
require 'optparse'

usage = "Usage: generate.rb -k path/to/private.key -i issuer-id [-u username]"

options = {}
OptionParser.new do |opts|
  opts.on("-h", "--help", "show help") do |h|
    puts usage
    exit
  end

  opts.on("-k KEY", "--key KEY", "Path to private key") do |key|
    options[:key] = key
  end

  opts.on("-i","--issuer-id issuer-id","Your Issuer Id") do |issuer_id|
    options[:issuer_id] = issuer_id
  end
  
  opts.on("-u","--username username", "A user name") do |username|
    options[:username] = username
  end
end.parse!

errors = []
[:key, :issuer_id].each do |key|
  unless options.has_key?(key)
    errors << "Option parameter %s is required" % key
  end
end

if errors.length > 0
  puts errors
  puts usage
  exit
end

begin
  rsa_private_key = OpenSSL::PKey::RSA.new File.read options[:key]
rescue Errno::ENOENT => e
  puts "Caught the exception: #{e}"
  puts usage
  exit -1
end

current_time = Time.now.to_i

payload = {}
payload['iss'] = options[:issuer_id]
payload['iat'] = current_time
payload['exp'] = current_time + 1800

if options.key?(:username)
  payload['sub'] = options[:username]
end

token = JWT.encode payload, rsa_private_key, 'RS512', { typ: 'JWT' }

puts token
# puts JWT.decode token, rsa_public_key, true, { :algorithm => 'RS512' }